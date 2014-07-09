using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Extensions
{
    public static class EntityExtensions
    {
        public static void AttachAndMarkModified(this DbContext context, ISharedRecord newVals)
        {
            AttachAndMarkModified(context, new ISharedRecord[] { newVals });
        }
        public static void AttachAndMarkModified(this DbContext context, IEnumerable<ISharedRecord> newVals)
        {
            var firstVal = newVals.FirstOrDefault();
            if (firstVal == null){return;}
            Type valType = firstVal.GetType(); 
            if (valType.Namespace == "System.Data.Entity.DynamicProxies") { valType = valType.BaseType; }
            var objContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext;
            var localDict = context.Set(valType).Local.Cast<ISharedRecord>().ToDictionary(k=>k.Id);
            
            string entitySetName = objContext.DefaultContainerName + '.' + GetEntitySetName(valType);

            foreach (ISharedRecord v in newVals)
            {
                ISharedRecord existingEntity;
                if (localDict.TryGetValue(v.Id, out existingEntity))
                {
                    var ent = context.Entry(existingEntity);
                    ent.CurrentValues.SetValues(v);
                    ent.State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    context.Set(valType).Attach(v);
                    context.Entry(v).State = EntityState.Modified;
                }
            }
            
        }
        static string GetEntitySetName(Type entityType)
        {
            if (entityType==typeof(VaccineAdministered))
            {
                return "VaccinesAdministered";
            }
            return entityType.Name + 's';
        }
    }
}
