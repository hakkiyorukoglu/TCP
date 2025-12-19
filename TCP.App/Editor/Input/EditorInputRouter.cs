using System;
using System.Windows;
using System.Windows.Input;

namespace TCP.App.Editor.Input;

/// <summary>
/// EditorPointerEvent - Mouse pointer event DTO
/// 
/// TCP-1.0.3: EditorInputRouter (single input gateway)
/// 
/// Mouse pointer event bilgilerini taşır.
/// </summary>
public class EditorPointerEvent
{
    /// <summary>
    /// Screen point (relative to editor surface)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public Point ScreenPoint { get; set; }
    
    /// <summary>
    /// Button that triggered the event (if applicable)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public MouseButton? Button { get; set; }
    
    /// <summary>
    /// All currently pressed buttons
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public MouseButtonState LeftButton { get; set; }
    public MouseButtonState RightButton { get; set; }
    public MouseButtonState MiddleButton { get; set; }
    
    /// <summary>
    /// Modifier keys state
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public bool Ctrl { get; set; }
    public bool Shift { get; set; }
    public bool Alt { get; set; }
    
    /// <summary>
    /// Event timestamp
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// EditorWheelEvent - Mouse wheel event DTO
/// 
/// TCP-1.0.3: EditorInputRouter (single input gateway)
/// </summary>
public class EditorWheelEvent
{
    /// <summary>
    /// Screen point (relative to editor surface)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public Point ScreenPoint { get; set; }
    
    /// <summary>
    /// Wheel delta (positive = scroll up, negative = scroll down)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public int Delta { get; set; }
    
    /// <summary>
    /// Modifier keys state
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public bool Ctrl { get; set; }
    public bool Shift { get; set; }
    public bool Alt { get; set; }
    
    /// <summary>
    /// Event timestamp
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// EditorKeyEvent - Keyboard event DTO
/// 
/// TCP-1.0.3: EditorInputRouter (single input gateway)
/// </summary>
public class EditorKeyEvent
{
    /// <summary>
    /// Key that triggered the event
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public Key Key { get; set; }
    
    /// <summary>
    /// Whether this is a repeat key press
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public bool IsRepeat { get; set; }
    
    /// <summary>
    /// Modifier keys state
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public bool Ctrl { get; set; }
    public bool Shift { get; set; }
    public bool Alt { get; set; }
    
    /// <summary>
    /// Event timestamp
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// EditorInputRouter - Single input gateway for Editor
/// 
/// TCP-1.0.3: EditorInputRouter (single input gateway)
/// 
/// Bu sınıf Editor için tüm mouse/keyboard input event'lerini toplar ve merkezi bir yerden yönetir.
/// Single Responsibility: Editor input event collection ve routing
/// 
/// IMPORTANT:
/// - Router ViewModel veya View'a direkt referans vermez (pure utility)
/// - Router herhangi bir sırada çağrılabilir (null check'ler, exception yok)
/// - TCP-1.0.3: Sadece event toplama, viewport değişikliği yok
/// </summary>
public class EditorInputRouter
{
    /// <summary>
    /// Last mouse screen position
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public Point LastMouseScreen { get; private set; }
    
    /// <summary>
    /// Currently pressed mouse buttons state
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public MouseButtonState LeftButton { get; private set; }
    public MouseButtonState RightButton { get; private set; }
    public MouseButtonState MiddleButton { get; private set; }
    
    /// <summary>
    /// Modifier keys state
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public bool IsCtrlDown { get; private set; }
    public bool IsShiftDown { get; private set; }
    public bool IsAltDown { get; private set; }
    
    /// <summary>
    /// Last wheel delta (for display/debug)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public int LastWheelDelta { get; private set; }
    
    /// <summary>
    /// Last key pressed (for display/debug)
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public Key? LastKey { get; private set; }
    
    /// <summary>
    /// Pointer moved event
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public event Action<EditorPointerEvent>? PointerMoved;
    
    /// <summary>
    /// Pointer down event
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public event Action<EditorPointerEvent>? PointerDown;
    
    /// <summary>
    /// Pointer up event
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public event Action<EditorPointerEvent>? PointerUp;
    
    /// <summary>
    /// Wheel changed event
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public event Action<EditorWheelEvent>? WheelChanged;
    
    /// <summary>
    /// Key down event
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public event Action<EditorKeyEvent>? KeyDown;
    
    /// <summary>
    /// Key up event
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public event Action<EditorKeyEvent>? KeyUp;
    
    /// <summary>
    /// On mouse move handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public void OnMouseMove(Point screenPoint, ModifierKeys modifiers, MouseButtonState leftButton, MouseButtonState rightButton, MouseButtonState middleButton)
    {
        try
        {
            // TCP-1.0.3: Update state
            LastMouseScreen = screenPoint;
            LeftButton = leftButton;
            RightButton = rightButton;
            MiddleButton = middleButton;
            IsCtrlDown = (modifiers & ModifierKeys.Control) != 0;
            IsShiftDown = (modifiers & ModifierKeys.Shift) != 0;
            IsAltDown = (modifiers & ModifierKeys.Alt) != 0;
            
            // TCP-1.0.3: Create event DTO
            var evt = new EditorPointerEvent
            {
                ScreenPoint = screenPoint,
                Button = null, // Move events don't have a specific button
                LeftButton = leftButton,
                RightButton = rightButton,
                MiddleButton = middleButton,
                Ctrl = IsCtrlDown,
                Shift = IsShiftDown,
                Alt = IsAltDown,
                Timestamp = DateTime.Now
            };
            
            // TCP-1.0.3: Fire event
            PointerMoved?.Invoke(evt);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse move
        }
    }
    
    /// <summary>
    /// On mouse down handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public void OnMouseDown(Point screenPoint, MouseButton button, ModifierKeys modifiers, MouseButtonState leftButton, MouseButtonState rightButton, MouseButtonState middleButton)
    {
        try
        {
            // TCP-1.0.3: Update state
            LastMouseScreen = screenPoint;
            LeftButton = leftButton;
            RightButton = rightButton;
            MiddleButton = middleButton;
            IsCtrlDown = (modifiers & ModifierKeys.Control) != 0;
            IsShiftDown = (modifiers & ModifierKeys.Shift) != 0;
            IsAltDown = (modifiers & ModifierKeys.Alt) != 0;
            
            // TCP-1.0.3: Create event DTO
            var evt = new EditorPointerEvent
            {
                ScreenPoint = screenPoint,
                Button = button,
                LeftButton = leftButton,
                RightButton = rightButton,
                MiddleButton = middleButton,
                Ctrl = IsCtrlDown,
                Shift = IsShiftDown,
                Alt = IsAltDown,
                Timestamp = DateTime.Now
            };
            
            // TCP-1.0.3: Fire event
            PointerDown?.Invoke(evt);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse down
        }
    }
    
    /// <summary>
    /// On mouse up handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public void OnMouseUp(Point screenPoint, MouseButton button, ModifierKeys modifiers, MouseButtonState leftButton, MouseButtonState rightButton, MouseButtonState middleButton)
    {
        try
        {
            // TCP-1.0.3: Update state
            LastMouseScreen = screenPoint;
            LeftButton = leftButton;
            RightButton = rightButton;
            MiddleButton = middleButton;
            IsCtrlDown = (modifiers & ModifierKeys.Control) != 0;
            IsShiftDown = (modifiers & ModifierKeys.Shift) != 0;
            IsAltDown = (modifiers & ModifierKeys.Alt) != 0;
            
            // TCP-1.0.3: Create event DTO
            var evt = new EditorPointerEvent
            {
                ScreenPoint = screenPoint,
                Button = button,
                LeftButton = leftButton,
                RightButton = rightButton,
                MiddleButton = middleButton,
                Ctrl = IsCtrlDown,
                Shift = IsShiftDown,
                Alt = IsAltDown,
                Timestamp = DateTime.Now
            };
            
            // TCP-1.0.3: Fire event
            PointerUp?.Invoke(evt);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse up
        }
    }
    
    /// <summary>
    /// On mouse wheel handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public void OnMouseWheel(Point screenPoint, int delta, ModifierKeys modifiers, MouseButtonState leftButton, MouseButtonState rightButton, MouseButtonState middleButton)
    {
        try
        {
            // TCP-1.0.3: Update state
            LastMouseScreen = screenPoint;
            LastWheelDelta = delta;
            LeftButton = leftButton;
            RightButton = rightButton;
            MiddleButton = middleButton;
            IsCtrlDown = (modifiers & ModifierKeys.Control) != 0;
            IsShiftDown = (modifiers & ModifierKeys.Shift) != 0;
            IsAltDown = (modifiers & ModifierKeys.Alt) != 0;
            
            // TCP-1.0.3: Create event DTO
            var evt = new EditorWheelEvent
            {
                ScreenPoint = screenPoint,
                Delta = delta,
                Ctrl = IsCtrlDown,
                Shift = IsShiftDown,
                Alt = IsAltDown,
                Timestamp = DateTime.Now
            };
            
            // TCP-1.0.3: Fire event
            WheelChanged?.Invoke(evt);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during mouse wheel
        }
    }
    
    /// <summary>
    /// On key down handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public void OnKeyDown(Key key, bool isRepeat, ModifierKeys modifiers)
    {
        try
        {
            // TCP-1.0.3: Update state
            LastKey = key;
            IsCtrlDown = (modifiers & ModifierKeys.Control) != 0;
            IsShiftDown = (modifiers & ModifierKeys.Shift) != 0;
            IsAltDown = (modifiers & ModifierKeys.Alt) != 0;
            
            // TCP-1.0.3: Create event DTO
            var evt = new EditorKeyEvent
            {
                Key = key,
                IsRepeat = isRepeat,
                Ctrl = IsCtrlDown,
                Shift = IsShiftDown,
                Alt = IsAltDown,
                Timestamp = DateTime.Now
            };
            
            // TCP-1.0.3: Fire event
            KeyDown?.Invoke(evt);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during key down
        }
    }
    
    /// <summary>
    /// On key up handler
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public void OnKeyUp(Key key, bool isRepeat, ModifierKeys modifiers)
    {
        try
        {
            // TCP-1.0.3: Update state
            IsCtrlDown = (modifiers & ModifierKeys.Control) != 0;
            IsShiftDown = (modifiers & ModifierKeys.Shift) != 0;
            IsAltDown = (modifiers & ModifierKeys.Alt) != 0;
            
            // TCP-1.0.3: Create event DTO
            var evt = new EditorKeyEvent
            {
                Key = key,
                IsRepeat = isRepeat,
                Ctrl = IsCtrlDown,
                Shift = IsShiftDown,
                Alt = IsAltDown,
                Timestamp = DateTime.Now
            };
            
            // TCP-1.0.3: Fire event
            KeyUp?.Invoke(evt);
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during key up
        }
    }
    
    /// <summary>
    /// Reset router state
    /// TCP-1.0.3: EditorInputRouter (single input gateway)
    /// </summary>
    public void Reset()
    {
        try
        {
            LastMouseScreen = new Point(0, 0);
            LeftButton = MouseButtonState.Released;
            RightButton = MouseButtonState.Released;
            MiddleButton = MouseButtonState.Released;
            IsCtrlDown = false;
            IsShiftDown = false;
            IsAltDown = false;
            LastWheelDelta = 0;
            LastKey = null;
        }
        catch
        {
            // TCP-1.0.3: Safety - ignore exceptions during reset
        }
    }
}
