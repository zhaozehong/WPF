using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.UserControls
{
  public enum KeyboardKeys
  {
    None = 0,

    // Digit
    D_D0,
    D_D1,
    D_D2,
    D_D3,
    D_D4,
    D_D5,
    D_D6,
    D_D7,
    D_D8,
    D_D9,

    // Operator
    O_Add,
    O_Substract,
    O_Multiply,
    O_Divide,

    // Function
    F_Abs,
    F_Sin,
    F_Cos,
    F_Tan,
    F_In,
    F_Sqrt,

    LeftBracket,
    RightBracket,

    Point,

    Backspace,
    AC,
    C,
    PI,

    Equal,
    Inv,

    M2I,
    I2M,

    Enter = 100,
    Pin,
    CLR,
    Close,
    SWITCH
  }
  public static class KeyboardHelper
  {
    public static Boolean IsFunctionKey(KeyboardKeys key)
    {
      return key.ToString().StartsWith("F_");
    }
    public static Boolean IsOperatorKey(KeyboardKeys key)
    {
      return key.ToString().StartsWith("O_");
    }
    public static Boolean IsDigitKey(KeyboardKeys key)
    {
      return key.ToString().StartsWith("D_");
    }
    public static Boolean IsSubmitKey(KeyboardKeys key)
    {
      return key == KeyboardKeys.Equal || key == KeyboardKeys.Enter;
    }
    public static Boolean IsPointKey(KeyboardKeys key)
    {
      return key == KeyboardKeys.Point;
    }
    public static Boolean IsLeftBracketKey(KeyboardKeys key)
    {
      return key == KeyboardKeys.LeftBracket;
    }
    public static Boolean IsRightBracketKey(KeyboardKeys key)
    {
      return key == KeyboardKeys.RightBracket;
    }
  }
}
