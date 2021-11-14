using System;
using System.Collections;
using System.Collections.Generic;

namespace LogsServer.Domain
{
    public class Log
    {
        public DateTime CreatedAt { get; set; }
        public LogTag LogTag { get; set; }
        public dynamic Entity { get; set; }
        public Type EntityType { get; set; }

        public bool IsEntityAList()
        {
            return EntityType.IsGenericType && EntityType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
        
        public override string ToString()
        {
            string result;
            
            if (IsEntityAList())
            {
                IList entityList = Entity as IList;
                result = $"[{CreatedAt}] ({LogTag}) - \n";
                
                foreach (var entity in entityList)
                {
                    result += $"{entity} \n";
                }
            }
            else
            {
                result = $"[{CreatedAt}] ({LogTag}) - {Entity.ToString()}";
            }

            return result;
        }
    }
}