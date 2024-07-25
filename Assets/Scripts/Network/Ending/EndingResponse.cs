using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EndingResponse
{
    public int code;
    public string msg;
    public List<GetRankResponse> data;
}
