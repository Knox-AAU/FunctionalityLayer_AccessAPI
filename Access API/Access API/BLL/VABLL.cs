﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Access_API.DAL;

namespace Access_API.BLL
{
    public class VABLL
    {
        public VAResultDTO vaBLL_getNode(string input)
        {
            string url = Urls.vaUrl + $"/knowledgeGraph/getNode?name={input}";
            
            VADAL DAL = new VADAL();

            return DAL.GetVAResults(url);
        }

        public VAResultDTO vaBLL_getNodes(string input)
        {
            string url = Urls.vaUrl + $"/knowledgeGraph/getNodes?name={input}";
            VADAL DAL = new VADAL();

            var res =  DAL.GetVAResults(url);
            return res;
        }
    }
}
