using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public partial class AreasSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysAreas.AnyAsync()) return;
            await context.SysAreas.AddRangeAsync(new List<SysArea>
            {
                new SysArea
                {
                    Id = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),


                    AreaName = "الرياض",

                },
                new SysArea
                {
                    Id = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),


                    AreaName = "مكة المكرمة",

                },
                new SysArea
                {
                    Id = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),


                    AreaName = "المدينة المنورة",

                },new SysArea
                {
                    Id = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),


                    AreaName = "القصيم",

                },new SysArea
                {
                    Id = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),


                    AreaName = "الشرقية",

                },new SysArea
                {
                    Id = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),


                    AreaName = "عسير",

                },new SysArea
                {
                    Id = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),


                    AreaName = "تبوك",

                },new SysArea
                {
                    Id = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),


                    AreaName = "حائل",

                },new SysArea
                {
                    Id = new Guid("86efb2d9-4b25-4e46-b13d-4ec1cf8048d7"),


                    AreaName = "الحدود الشمالية",

                },new SysArea
                {
                    Id = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),


                    AreaName = "جازان",

                },new SysArea
                {
                    Id = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),


                    AreaName = " نجران",

                },new SysArea
                {
                    Id = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),


                    AreaName = "الباحة",

                },new SysArea
                {
                    Id = new Guid("7355b4f0-9271-46ca-88bd-3c0eb8bef67a"),


                    AreaName = "الجوف",

                },

            }
            );
            context.SaveChanges();

        }
    }
}