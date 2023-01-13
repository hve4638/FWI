namespace FWIConnection
{
    public enum MessageOp : short
    {
        None = 0,
        Echo = 1,
        Message = 2,

        UpdateCurrentWI = 10,
        ResetCurrent = 11,
        SetAFK = 12,
        UnsetAFK = 13,

        SetAlias = 20,
        SetGroup = 21,
        RequestServerInfo = 30,
        RequestTimeline = 31,
        RequestRank = 32,

        RequestPrivillegeTrace = 40,

        ResponsePrivillegeTrace = 50,

        Execute = 90,
        Alert = 91,
        Debug = 99,
    }
}
