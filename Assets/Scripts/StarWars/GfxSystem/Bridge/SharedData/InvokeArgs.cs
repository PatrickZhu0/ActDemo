﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public class TemporaryEffectArgs
    {
        public TemporaryEffectArgs(string resource, float duration, float x, float y, float z)
        {
            Resource = resource;
            Duration = duration;
            X = x;
            Y = y;
            Z = z;
        }

        public string Resource;
        public float Duration;
        public float X;
        public float Y;
        public float Z;
    }

    public class SkillParam
    {
        public int TargetId;
        public int SkillId;
    }
    public class GestureArgs
    {
        public GestureArgs()
        {
        }
        /// 开始位置
        public float startPositionX;
        public float startPositionY;
        /// 当前位置
        public float positionX;
        public float positionY;
        /// 开始时间
        public float startTime;
        /// 花费时间
        public float elapsedTime;
        /// 开始游戏坐标
        public float startGamePosX;
        public float startGamePosY;
        public float startGamePosZ;
        /// 游戏中坐标
        public float gamePosX;
        public float gamePosY;
        public float gamePosZ;
        /// 游戏中寻路使用的坐标
        public float airWelGamePosX;
        public float airWelGamePosY;
        public float airWelGamePosZ;
        /// 手势名称
        public string name;
        /// 手势方向
        public float towards;
        /// 移动类型
        public TouchType moveType;
        /// 是否选中目标
        public int selectedObjID;
        /// 段数
        public int sectionNum;
        /// 技能攻击范围
        public float attackRange;
        /// 输入类型
        public InputType inputType;
    }

    public enum GestureEvent : int
    {
        OnLine = 0,
        OnEasyGesture = 1,
        OnFingerDown = 100,
        OnFingerMove = 101,
        OnFingerUp = 102,
        OnTwoFingerDown = 103,
        OnTwoFingerMove = 104,
        OnTwoFingerUp = 105,
        OnSingleTap = 201,
        OnDoubleTap = 202,
        OnLongPress = 203,
        OnSkillStart = 1000,
    }

    public enum TouchType : int
    {
        Move = 0,
        Regognizer,
        Attack,
    }

    public enum TouchEvent : int
    {
        Cesture = 0,
    }

    public enum InputType : int
    {
        Touch = 0,
        Joystick = 1,
    }

    public static class Keyboard
    {
        public enum Event
        {
            Up,
            Down,
            LongPressed,
        }
        //此枚举值需要与所用引擎的枚举定义一致
        public enum Code
        {
            None,
            Backspace = 8,
            Delete = 127,
            Tab = 9,
            Clear = 12,
            Return,
            Pause = 19,
            Escape = 27,
            Space = 32,
            Keypad0 = 256,
            Keypad1,
            Keypad2,
            Keypad3,
            Keypad4,
            Keypad5,
            Keypad6,
            Keypad7,
            Keypad8,
            Keypad9,
            KeypadPeriod,
            KeypadDivide,
            KeypadMultiply,
            KeypadMinus,
            KeypadPlus,
            KeypadEnter,
            KeypadEquals,
            UpArrow,
            DownArrow,
            RightArrow,
            LeftArrow,
            Insert,
            Home,
            End,
            PageUp,
            PageDown,
            F1,
            F2,
            F3,
            F4,
            F5,
            F6,
            F7,
            F8,
            F9,
            F10,
            F11,
            F12,
            F13,
            F14,
            F15,
            Alpha0 = 48,
            Alpha1,
            Alpha2,
            Alpha3,
            Alpha4,
            Alpha5,
            Alpha6,
            Alpha7,
            Alpha8,
            Alpha9,
            Exclaim = 33,
            DoubleQuote,
            Hash,
            Dollar,
            Ampersand = 38,
            Quote,
            LeftParen,
            RightParen,
            Asterisk,
            Plus,
            Comma,
            Minus,
            Period,
            Slash,
            Colon = 58,
            Semicolon,
            Less,
            Equals,
            Greater,
            Question,
            At,
            LeftBracket = 91,
            Backslash,
            RightBracket,
            Caret,
            Underscore,
            BackQuote,
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            K,
            L,
            M,
            N,
            O,
            P,
            Q,
            R,
            S,
            T,
            U,
            V,
            W,
            X,
            Y,
            Z,
            Numlock = 300,
            CapsLock,
            ScrollLock,
            RightShift,
            LeftShift,
            RightControl,
            LeftControl,
            RightAlt,
            LeftAlt,
            LeftCommand = 310,
            LeftApple = 310,
            LeftWindows,
            RightCommand = 309,
            RightApple = 309,
            RightWindows = 312,
            AltGr,
            Help = 315,
            Print,
            SysReq,
            Break,
            Menu,
            Mouse0 = 323,
            Mouse1,
            Mouse2,
            Mouse3,
            Mouse4,
            Mouse5,
            Mouse6,
            JoystickButton0,
            JoystickButton1,
            JoystickButton2,
            JoystickButton3,
            JoystickButton4,
            JoystickButton5,
            JoystickButton6,
            JoystickButton7,
            JoystickButton8,
            JoystickButton9,
            JoystickButton10,
            JoystickButton11,
            JoystickButton12,
            JoystickButton13,
            JoystickButton14,
            JoystickButton15,
            JoystickButton16,
            JoystickButton17,
            JoystickButton18,
            JoystickButton19,
            Joystick1Button0,
            Joystick1Button1,
            Joystick1Button2,
            Joystick1Button3,
            Joystick1Button4,
            Joystick1Button5,
            Joystick1Button6,
            Joystick1Button7,
            Joystick1Button8,
            Joystick1Button9,
            Joystick1Button10,
            Joystick1Button11,
            Joystick1Button12,
            Joystick1Button13,
            Joystick1Button14,
            Joystick1Button15,
            Joystick1Button16,
            Joystick1Button17,
            Joystick1Button18,
            Joystick1Button19,
            Joystick2Button0,
            Joystick2Button1,
            Joystick2Button2,
            Joystick2Button3,
            Joystick2Button4,
            Joystick2Button5,
            Joystick2Button6,
            Joystick2Button7,
            Joystick2Button8,
            Joystick2Button9,
            Joystick2Button10,
            Joystick2Button11,
            Joystick2Button12,
            Joystick2Button13,
            Joystick2Button14,
            Joystick2Button15,
            Joystick2Button16,
            Joystick2Button17,
            Joystick2Button18,
            Joystick2Button19,
            Joystick3Button0,
            Joystick3Button1,
            Joystick3Button2,
            Joystick3Button3,
            Joystick3Button4,
            Joystick3Button5,
            Joystick3Button6,
            Joystick3Button7,
            Joystick3Button8,
            Joystick3Button9,
            Joystick3Button10,
            Joystick3Button11,
            Joystick3Button12,
            Joystick3Button13,
            Joystick3Button14,
            Joystick3Button15,
            Joystick3Button16,
            Joystick3Button17,
            Joystick3Button18,
            Joystick3Button19,
            Joystick4Button0,
            Joystick4Button1,
            Joystick4Button2,
            Joystick4Button3,
            Joystick4Button4,
            Joystick4Button5,
            Joystick4Button6,
            Joystick4Button7,
            Joystick4Button8,
            Joystick4Button9,
            Joystick4Button10,
            Joystick4Button11,
            Joystick4Button12,
            Joystick4Button13,
            Joystick4Button14,
            Joystick4Button15,
            Joystick4Button16,
            Joystick4Button17,
            Joystick4Button18,
            Joystick4Button19,
            MaxNum
        }
    }
    public static class Mouse
    {
        public enum Event
        {
            Up = 0,
            Down,
            LongPressed
        }
        public enum Code
        {
            LeftButton = 0,
            MiddleButton,
            RightButton,
            NumMouseButtons,
            InvalidMouseButton,
            TheMouse = InvalidMouseButton,
            MaxNum
        }
    }
}

