using Godot;
using System;

public partial class Joystick : Control
{
    [Export] private float _deadZone = 0.2f;
    [Export] private float _clampZone = 0.9f;

    private Vector2 _handlerPosition;
    private Vector2 _inputVector = Vector2.Zero;
    private bool _isPonted = false;
    private TextureRect _background;
    private TextureRect _handler;

    /// <summary>
    /// Получение узлов джойстика, установление позиции по умолчанию для стика и скрытие джойстика
    /// </summary>
    public override void _Ready()
    {
        _background = GetNode<TextureRect>("Background");
        _handler = GetNode<TextureRect>("Background/Handler");
        _handlerPosition = _background.Position + _background.Size / 2 - _handler.Size / 2;
        Hide();
    }

    /// <summary>
    /// Обработка появления и скрытия джойстика при косании и отпускании
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed && !_isPonted)
            {
                if (touchEvent.Position.X < GetViewportRect().Size.X / 2)
                    Enable(touchEvent.Position);
            }
            else if (!touchEvent.Pressed && _isPonted)
                Disable();
        }
        else if (@event is InputEventScreenDrag dragEvent && _isPonted)
            UpdateHandlerPosition(dragEvent.Position);
    }

    /// <summary>
    /// Создание движения стика внутри джойстика и получения направления движения
    /// </summary>
    /// <param name="position">местоположение точки косания экрана</param>
    private void UpdateHandlerPosition(Vector2 position)
    {
        var radius = _background.Size.X / 2;
        var center = _handlerPosition;
        var relative = position - center;

        var length = MathF.Min(relative.Length(), radius * _clampZone);
        var direction = relative.Normalized();
        _inputVector = direction * (length / (radius * _clampZone));

        if (_inputVector.Length() < _deadZone)
            _inputVector = Vector2.Zero;
        _handler.Position = _background.Size / 2 + direction * length - _handler.Size / 2;
    }

    /// <summary>
    /// Показ джойстика в точке косания и установление позиции стика
    /// </summary>
    /// <param name="position"></param>
    private void Enable(Vector2 position)
    {
        _background.GlobalPosition = position - _background.Size / 2;
        _handler.Position = _background.Size / 2 - _handler.Size / 2;
        _handlerPosition = position;
        _inputVector = Vector2.Zero;
        _isPonted = true;
        Show();
    }

    /// <summary>
    /// Сокрытие джойстика при отсутствии точки косания
    /// </summary>
    private void Disable()
    {
        _inputVector = Vector2.Zero;
        _isPonted = false;
        Hide();
    }

    /// <summary>
    /// Получения направления движения для использования другими объектами
    public Vector2 InputVector => _inputVector;
}
