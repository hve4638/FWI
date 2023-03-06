namespace FWI.Message
{
    public enum MessageOp : short
    {
        None = 0,
        Echo = 1,
        Message = 2,

        UpdateWI = 10,
        ResetCurrent = 11,
        SetAFK = 12,
        SetNoAFK = 13,

        SetAlias = 20,
        SetGroup = 21,
        RequestServerInfo = 30,
        RequestTimeline = 31,
        RequestRank = 32,

        RequestToBeTarget = 40,
        ResponseToBeTarget = 50,

        Execute = 90,
        Alert = 91,
        ServerCall = 98,
        Debug = 99,

        RequestAlive = 100,
        ResponseAlive = 200,

        RequestEcho = 101,
        ResponseEcho = 201,

        //RequestRank = 111,
        ResponseRank = 211,
        ResponseTimeline = 212,


        //Obsolete
        UpdateCurrentWI = 1001,
        RequestPrivillegeTrace = 1002,
        ResponsePrivillegeTrace = 1003,
    }
}
