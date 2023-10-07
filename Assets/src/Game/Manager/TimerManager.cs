public class TimerManager
{
    public delegate void OnTimeEventHandler();

    public static event OnTimeEventHandler OnHourChanged;
    public static event OnTimeEventHandler OnDayChanged;
    public static event OnTimeEventHandler OnWeekChanged;
    public static event OnTimeEventHandler OnMonthChanged;
    public static event OnTimeEventHandler OnYearChanged;

    public static float secondEquivalenceHour = 20f;

    public static void setDefault()
    {
        _hour = 9;
        _day = 1;
        _weekday = 2;
        _month = 5;
        _year = 2023;
    }

    public static void setTime(int _h, int _d, int _w, int _m, int _y)
    {
        _hour = _h;
        _day = _d;
        _weekday = _w;
        _month = _m;
        _year = _y;
    }

    private static int _hour = 6;
    public static int hour
    {
        get { return _hour; }
        set
        {


            _hour = value;
            // hora trocada
            if (OnHourChanged != null)
                OnHourChanged.Invoke();

            if (_hour >= 24)
            {
                day++;
            }
        }
    }

    private static int _day = 1;
    public static int day
    {
        get { return _day; }
        set
        {

            hour = 0;
            _day = value;


            weekday++;

            if (_day > TimerManager.endMonth())
            {
                _day = 1;

                if (OnDayChanged != null)
                    OnDayChanged.Invoke();

                month++;
            }
            else
            {
                if (OnDayChanged != null)
                    OnDayChanged.Invoke();
            }
        }
    }

    private static int _weekday = 4;
    public static int weekday
    {
        get { return _weekday; }
        set
        {
            _weekday = value;
            if (_weekday > 7)
            {
                if (OnWeekChanged != null)
                    OnWeekChanged.Invoke();
                _weekday = 1;
            }
        }
    }


    private static int _month = 1;
    public static int month
    {
        get { return _month; }
        set
        {

            _month = value;

            if (_month > 12)
            {
                _month = 1;
                if (OnMonthChanged != null)
                    OnMonthChanged.Invoke();
                year++;
            }
            else
            {
                if (OnMonthChanged != null)
                    OnMonthChanged.Invoke();
            }

        }
    }

    private static int _year = 2014;
    public static int year
    {
        get { return _year; }
        set
        {

            _year = value;
            if (OnYearChanged != null)
                OnYearChanged.Invoke();
        }
    }

    public static int endMonth()
    {
        int end = 0;
        switch (TimerManager._month)
        {
            case 1:
            case 3:
            case 5:
            case 7:
            case 8:
            case 10:
            case 12:
                end = 31;
                break;
            case 2:
                // regra de bissexto
                if (_year % 4 == 0)
                {
                    if (_year % 100 == 0)
                    {
                        if (_year % 400 == 0)
                        {
                            end = 29;
                        }
                    }
                    else
                    {
                        end = 29;
                    }
                }
                else
                {
                    end = 28;
                }
                break;
            default:
                end = 30;
                break;
        }
        return end;
    }
}
