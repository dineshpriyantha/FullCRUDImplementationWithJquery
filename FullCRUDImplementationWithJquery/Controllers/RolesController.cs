using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using FullCRUDImplementationWithJquery.Models;
using System.Data.SqlClient;
using System.Data;
using FullCRUDImplementationWithJquery.DataAccess;

namespace FullCRUDImplementationWithJquery.Controllers
{
    public class RolesController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        //APN user list
        private List<APN_IdentityRole> role = new List<APN_IdentityRole>();
        private DataTable dt = new DataTable();

        //Db reference
        private DatabaseConnection db = new DatabaseConnection();
        // GET: Roles

        public RolesController()
        {
            SqlConnection conn = db.GetConnection();
            SqlDataAdapter da = new SqlDataAdapter("pr_APN_Roles", conn);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            DataSet dat_set = new DataSet();
            da.Fill(dat_set);
            conn.Close();

            role = dat_set.Tables[0].AsEnumerable().Select(
                                DataRow => new APN_IdentityRole
                                {
                                    Id = DataRow.Field<int>("Id"),
                                    Name = DataRow.Field<string>("Name")
                                }).ToList();

        }

        public ActionResult Index()
        {
            //var roles = //context.Roles.ToList();
            return View(role);
        }

        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                SqlConnection conn = db.GetConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Execute pr_APN_AddRoles @name";
                cmd.Parameters.Add("@name", SqlDbType.VarChar, -1).Value = collection["RoleName"];
                cmd.ExecuteNonQuery();
                conn.Close();

                ViewBag.ResultMessage = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(int roleId)
        {
            var thisRole = role.Where(r => r.Id == roleId).FirstOrDefault();

            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(APN_IdentityRole role)
        {
            try
            {
                SqlConnection conn = db.GetConnection();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Execute pr_APN_UpdateRoles @id,@name";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = role.Id;
                cmd.Parameters.Add("@name", SqlDbType.VarChar, -1).Value = role.Name;
                cmd.ExecuteNonQuery();
                conn.Close();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ManageUserRoles()
        {
            // prepopulat roles for the view dropdown
            var list = role.OrderBy(r => r.Name).ToList().Select(rr =>

            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            return View();
        }
        public ActionResult Delete(int RoleId)
        {
            SqlConnection conn = db.GetConnection();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "Execute pr_APN_DeleteRoles @id";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = RoleId;
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            SqlConnection conn = db.GetConnection();
            SqlCommand cmd = new SqlCommand("pr_APN_RoleAddToUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            cmd.Parameters.Add("@msg", SqlDbType.VarChar, 100);
            cmd.Parameters["@msg"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@color", SqlDbType.VarChar, 20);
            cmd.Parameters["@color"].Direction = ParameterDirection.Output;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            ViewBag.ResultMessage = cmd.Parameters["@msg"].Value.ToString();
            ViewBag.color = cmd.Parameters["@color"].Value.ToString();

            conn.Close();

            // prepopulat roles for the view dropdown
            var list = role.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            List<string> roles = new List<string>();
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                SqlConnection conn = db.GetConnection();
                SqlCommand cmd = new SqlCommand("pr_APN_GetRolesforUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 100);
                cmd.Parameters["@msg"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@color", SqlDbType.VarChar, 20);
                cmd.Parameters["@color"].Direction = ParameterDirection.Output;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                ViewBag.NotRolesForThisUser = cmd.Parameters["@msg"].Value.ToString();
                ViewBag.color = cmd.Parameters["@color"].Value.ToString();

                if (dt.Rows.Count > 0)
                {
                    foreach (var row in dt.AsEnumerable())
                    {
                        roles.Add(row.Field<string>("RoleName"));
                    }
                    ViewBag.RolesForThisUser = roles;
                }

                conn.Close();

                var list = role.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
            }

            return View("ManageUserRoles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            SqlConnection conn = db.GetConnection();
            SqlCommand cmd = new SqlCommand("pr_APN_DeleteRoleForUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@RoleName", RoleName);
            cmd.Parameters.Add("@msg", SqlDbType.VarChar, 100);
            cmd.Parameters["@msg"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@color", SqlDbType.VarChar, 20);
            cmd.Parameters["@color"].Direction = ParameterDirection.Output;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            ViewBag.DeleteRoleResultMessage = cmd.Parameters["@msg"].Value.ToString();
            ViewBag.color = cmd.Parameters["@color"].Value.ToString();

            conn.Close();

            var list = role.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }
    }
}