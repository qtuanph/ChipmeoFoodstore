using System;

namespace ChipmeoApis.Core.Utils;

public static class TimeUtils
{
    public static DateTime GetVietnamTime()
    {
        
        string[] timeZoneIds = { "SE Asia Standard Time", "Asia/Ho_Chi_Minh" };

        foreach (var id in timeZoneIds)
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var tz))
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            }
        }

        // Fallback: UTC+7 (Vi?t Nam kh�ng c� DST n�n c?ng tay v?n d�ng)
        return DateTime.UtcNow.AddHours(7);
    }
}




