using System;
public static class DateUtils
{

    public static int CompareDates(string date1, string date2)
    {
        bool isDate1Valid = DateTime.TryParse(date1, out DateTime dateTime1);
        bool isDate2Valid = DateTime.TryParse(date2, out DateTime dateTime2);
        if (!isDate1Valid || !isDate2Valid) return -2;
        return dateTime1.CompareTo(dateTime2);
    }
}