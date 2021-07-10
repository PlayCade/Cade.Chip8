using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Xml;
using Cade.Common.Interfaces;
using Veldrid;
using Veldrid.SPIRV;

namespace Cade.Chip8
{
    public class Chip8OutputManager : CadeOutputManager
    {
        private CommandList _commandList;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;
        private Shader[] _shaders;
        private Pipeline _pipeline;

        private byte[]? VertexCode;
        private byte[]? FragmentCode;

        public Chip8OutputManager(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
            Graphics = new byte[64 * 32];
        }
        public (byte[] FragementShader, byte[] VertexShader) GetShaders()
        {
            var fragmentShader = "Cade.Chip8.Shaders.fragment.glsl";
            var vertexShader = "Cade.Chip8.Shaders.vertex.glsl";
            
            var assembly = Assembly.GetExecutingAssembly();
            var fragmentStream = assembly.GetManifestResourceStream(fragmentShader);
            var vertexStream = assembly.GetManifestResourceStream(vertexShader);
 
            if (fragmentStream == null)
            {
                throw new FileNotFoundException("Cannot find mappings file.", fragmentShader);
            }
            
            if (vertexStream == null)
            {
                throw new FileNotFoundException("Cannot find mappings file.", vertexShader);
            }

            byte[] fragmentBytes = new byte[fragmentStream.Length];
            using MemoryStream fms = new(fragmentBytes);
            fragmentStream.CopyTo(fms);

            byte[] vertexBytes = new byte[vertexStream.Length];
            using MemoryStream vms = new(vertexBytes);
            vertexStream.CopyTo(vms);

            return (fragmentBytes, vertexBytes);
        }

        public override void Setup()
        {
            (FragmentCode, VertexCode) = GetShaders();
            var factory = _graphicsDevice.ResourceFactory;

            _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColour.SizeInBytes * 64 * 32 , BufferUsage.VertexBuffer));
            _indexBuffer = factory.CreateBuffer(new BufferDescription(6 * sizeof(ushort) * 64 * 32, BufferUsage.IndexBuffer));

            var vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float2),
                new VertexElementDescription("Colour", VertexElementSemantic.TextureCoordinate,
                    VertexElementFormat.Float4));

            var vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                VertexCode,
                "main");
            var fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                FragmentCode,
                "main");

            _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            var pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription
            (
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual
            );
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false
            );
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
            pipelineDescription.ResourceLayouts = Array.Empty<ResourceLayout>();
            
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);
            
            pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;
            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            
            _commandList = factory.CreateCommandList();
        }

        public override void Draw()
        {
            var sprite = RgbaFloat.White;
            var background = RgbaFloat.Black;

            var quadVertices = new List<VertexPositionColour>();

            var quadIndices = new List<ushort>();
            var spriteWidth = 2f / 64f;
            var spriteHeight = 2f / 32f;
            
            for (float i = 0; i < 64 * 32; i++)
            {
                if (Graphics[(int)i / 64 * 64 + (int)i % 64] != 1) continue;
                
                var rowNumber = (int)(i / 64);
                var row = rowNumber * spriteHeight;
                
                var topLeft = new Vector2(i % 64 * spriteWidth - 1f, 1f - row);
                var topRight = new Vector2( i % 64 * spriteWidth + spriteWidth + -1f, 1f - row);
                var bottomLeft = new Vector2(i % 64 * spriteWidth - 1f, 1f - row - spriteHeight );
                var bottomRight = new Vector2(i % 64 * spriteWidth + spriteWidth + -1f, 1f - row - spriteHeight);

                var count = quadVertices.Count;
                
                quadVertices.Add(new(topLeft, sprite));
                quadVertices.Add(new(topRight, sprite));
                quadVertices.Add(new(bottomLeft, sprite));
                quadVertices.Add(new(bottomRight, sprite));

                quadIndices.Add((ushort)(count + 0));
                quadIndices.Add((ushort)(count + 1));
                quadIndices.Add((ushort)(count + 2)); 
                quadIndices.Add((ushort)(count + 1));
                quadIndices.Add((ushort)(count + 3));
                quadIndices.Add((ushort)(count + 2));
            }

            _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices.ToArray());
            _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices.ToArray());
            
            _commandList.Begin();
            _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            _commandList.ClearColorTarget(0, background);
            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetPipeline(_pipeline);
            _commandList.DrawIndexed(
                indexCount: (uint)quadIndices.Count,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
            _graphicsDevice.SwapBuffers();
        }

        public byte[] Graphics { get; set; }

        public override void Dispose()
        {
            _pipeline.Dispose();
            _commandList.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
    
    struct VertexPositionColour
    {
        public Vector2 Position;
        public RgbaFloat Colour;

        public VertexPositionColour(Vector2 position, RgbaFloat colour)
        {
            Position = position;
            Colour = colour;
        }

        public const uint SizeInBytes = 24;
    }
}