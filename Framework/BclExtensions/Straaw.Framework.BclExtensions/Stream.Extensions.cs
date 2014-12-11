using System.ComponentModel;
using System.IO;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class StreamExtensions
{
    #region Operations

    public static byte[] ReadAllBytes(this Stream stream)
    {
        var data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        return data;
    }

    #endregion
}
