using System;

public static class Constants
{
    //Constants
    public static UInt16 USERNAME_LENGTH_MIN { get { return 3; } }
    public static UInt16 USERNAME_LENGTH_MAX { get { return 10; } }

    //Error codes
    public static UInt16 USERNAME_BAD { get { return 0x00; } }
    public static UInt16 USERNAME_OK { get { return 0x01; } }
}