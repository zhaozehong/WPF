using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexagon.Software.NCGage.HelperLib
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

    D_Flag,
    D_PI,
    Point,

    // Operator
    O_Add,
    O_Substract,
    O_Multiply,
    O_Divide,
    O_Mod,
    O_Square,

    // Function
    F_Abs,
    F_Sin,
    F_Cos,
    F_Tan,
    F_ln,
    F_Sqrt,
    F_ASin,
    F_ACos,
    F_ATan,
    F_Exp,
    // Extra Functions ////////
    F_Ceiling,
    F_Floor,
    F_Truncate,
    F_Sign,
    F_Log,
    F_Log10,
    F_Sinh,
    F_Cosh,
    F_Tanh,
    //Complex
    F_Max,
    F_Min,
    F_Average,
    F_Ave,
    F_Sum,
    F_Pow,
    F_Atan2,
    F_LogX,
    F_IEEERemainder,
    M2I,
    I2M,
    ///////////////////////////

    LeftBracket,
    RightBracket,


    // Control
    Backspace,
    AC,
    C,

    Equal,
    Inv,



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
    public static Boolean IsEqualKey(KeyboardKeys key)
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
    public static Boolean IsUnitConverterKey(KeyboardKeys key)
    {
      return key == KeyboardKeys.M2I || key == KeyboardKeys.I2M;
    }
    public static List<String> FunctionNames = Enum.GetNames(typeof(KeyboardKeys)).Where(p => p.StartsWith("F_")).Select(p => p.Substring(2)).ToList();
  }
}
