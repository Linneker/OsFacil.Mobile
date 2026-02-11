using System;
using System.Collections.Generic;
using System.Text;

namespace OsFacil.Mobile.Service.Util;

public class ResponseHttps<T> where T : IResponseHttp
{
    public T Data { get; set; }
    public string Error { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public int StatusCode { get; set; }
}
public class ResponsesHttps<T> where T : IResponseHttp
{
    public List<T> Data { get; set; }
    public string Error { get; set; }
    public bool IsSuccessStatusCode { get; set; }
    public int StatusCode { get; set; }
}
