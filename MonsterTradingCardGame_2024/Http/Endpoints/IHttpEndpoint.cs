using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal interface IHttpEndpoint
    {
        bool HandleRequest(HttpRequest rq, HttpResponse rs);
    }
}
