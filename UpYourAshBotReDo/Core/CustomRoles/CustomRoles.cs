using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpYourAshBotReDo.Core.CustomRoles
{
    public class CustomRoles
    {
        public List<CustomRole> customRoles;

        public string filePath;

        public CustomRoles(string _filePath)
        {
            filePath = _filePath;

            if (DataStorage.SaveExists(filePath))
            {
                customRoles = DataStorage.LoadCustomRoles(filePath).ToList();
                //TODO - Verafiy Roles
            }
            else
            {
                customRoles = new List<CustomRole>();
                SaveRoles();
            }
        }
        
        //public CustomRole GetRole(ulong id)
        //{
        //    var result = from a in customRoles
        //                 where a.ID == id
        //                 select a;

        //    var role = result.FirstOrDefault();
        //    if (role == null) return null;
        //    return role;
        //}

        //public CustomRole GetRole(string name)
        //{
        //    var result = from a in customRoles
        //                 where a.Name.ToLower() == name.ToLower()
        //                 select a;

        //    var role = result.FirstOrDefault();
        //    if (role == null) return null;
        //    return role;
        //}

        public CustomRole GetRole(string var)
        {
            CustomRole cR;

            ulong ID = 0;
            ulong.TryParse(var, out ID);

            if (ID != 0)
            {
                var result = from a in customRoles
                             where a.ID == ID
                             select a;

                cR = result.FirstOrDefault();
            }
            else
            {
                var result = from a in customRoles
                             where a.Name.ToLower() == var.ToLower()
                             select a;

                cR = result.FirstOrDefault();
            }

            return cR;
        }

        public void SaveRoles()
        {
            DataStorage.SaveCustomRoles(customRoles, filePath);
        }
    }
}
