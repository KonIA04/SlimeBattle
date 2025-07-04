using Godot;
using System;

public partial class TestChar : TextureRect
{
    
    
        [Export] public float Speed = 200;
        private Joystick _joyStick;

        public override void _Ready()
        {
            _joyStick = GetNode<Joystick>("Joystick");
        }

        public override void _Process(double delta)
        {
            var input = _joyStick.InputVector;
            Position += input;
        }

    
}
