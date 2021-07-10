using System;
using System.Collections.Generic;
using Cade.Common.Interfaces;
using Cade.Common.Models;

namespace Cade.Chip8
{
    public class Chip8InputManager : CadeInputManager
    {
        private readonly byte[] _keys = new byte[16];
        
        private static Lazy<List<Input>> _lazy = new(() => new List<Input>
        {
            new Input
                {
                    Name = "Button 0",
                    Active = false,
                    Number = 1,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 1",
                    Active = false,
                    Number = 2,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 2",
                    Active = false,
                    Number = 3,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 3",
                    Active = false,
                    Number = 4,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 4",
                    Active = false,
                    Number = 5,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 5",
                    Active = false,
                    Number = 6,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 6",
                    Active = false,
                    Number = 7,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 7",
                    Active = false,
                    Number = 8,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 8",
                    Active = false,
                    Number = 9,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button 9",
                    Active = false,
                    Number = 10,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button A",
                    Active = false,
                    Number = 11,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button B",
                    Active = false,
                    Number = 12,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button C",
                    Active = false,
                    Number = 13,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button D",
                    Active = false,
                    Number = 14,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button E",
                    Active = false,
                    Number = 15,
                    Player = 1,
                    Type = InputType.Button
                },
                new Input
                {
                    Name = "Button F",
                    Active = false,
                    Number = 16,
                    Player = 1,
                    Type = InputType.Button
                }
        });

        public override List<Input> Inputs()
        {
            return _lazy.Value;
        }

        public override void Update(List<Input> inputs)
        {
            _lazy = new Lazy<List<Input>>(() => inputs);
        }

        public override int MaxPlayers()
        {
            return 1;
        }
        
        public byte[] CheckKeys()
        {
            var inputs = _lazy.Value;

            foreach (var input in inputs)
            {
                switch (input.Number)
                {
                    case 1:
                        _keys[0x0] = Convert.ToByte(input.Active);
                        break;
                    case 2:
                        _keys[0x1] = Convert.ToByte(input.Active);
                        break;
                    case 3:
                        _keys[0x2] = Convert.ToByte(input.Active);
                        break;
                    case 4:
                        _keys[0x3] = Convert.ToByte(input.Active);
                        break;
                    case 5:
                        _keys[0x4] = Convert.ToByte(input.Active);
                        break;
                    case 6:
                        _keys[0x5] = Convert.ToByte(input.Active);
                        break;
                    case 7:
                        _keys[0x6] = Convert.ToByte(input.Active);
                        break;
                    case 8:
                        _keys[0x7] = Convert.ToByte(input.Active);
                        break;
                    case 9:
                        _keys[0x8] = Convert.ToByte(input.Active);
                        break;
                    case 10:
                        _keys[0x9] = Convert.ToByte(input.Active);
                        break;
                    case 11:
                        _keys[0xA] = Convert.ToByte(input.Active);
                        break;
                    case 12:
                        _keys[0xB] = Convert.ToByte(input.Active);
                        break;
                    case 13:
                        _keys[0xC] = Convert.ToByte(input.Active);
                        break;
                    case 14:
                        _keys[0xD] = Convert.ToByte(input.Active);
                        break;
                    case 15:
                        _keys[0xE] = Convert.ToByte(input.Active);
                        break;
                    case 16:
                        _keys[0xF] = Convert.ToByte(input.Active);
                        break;
                }
            }
            return _keys;
        }
    }
}