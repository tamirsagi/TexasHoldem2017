using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public enum StateMachine
    {
        SetIP,
        MainWindow,
        Login,
        ChooseTable,
        CreateTable,
        Table,
        Bets,
        Credits,
        Help,
        Kick
    };
}
