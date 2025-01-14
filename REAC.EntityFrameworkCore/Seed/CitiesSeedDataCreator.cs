using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public partial class CitiesSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysCities.AnyAsync()) return;
            await context.SysCities.AddRangeAsync(new List<SysCity>
            {
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الشرقية",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "القصيم",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الاحساء",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "جيزان",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الجولف",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },

                #region الرياض
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الرياض",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الدرعية",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الخرج",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الدوادمي",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "المجمعة",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "القويعية",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "الأفلاج",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),
                    CityName = "وادي الدواسر",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الزلفي",
                    //ChambersOfCommerceCityCode = 103,
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "شقراء",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "حوطة بني تميم",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "عفيف",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الغاط",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "السليل",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ضرما",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المزاحمية",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "رماح",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ثادق",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "حريملاء",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الحريق",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "مرات",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الرين",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الدلم",
                    AreaId = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),

                },

                #endregion

                #region مكة المكرمة
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "مكة المكرمة",
                    //ChambersOfCommerceCityCode = 202,
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "جده",
                    //ChambersOfCommerceCityCode = 201,
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الطائف",
                    //ChambersOfCommerceCityCode = 204,
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "القنفذة",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الليث",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "رابغ",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "خليص",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الخرمة",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "رنية",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "تربة",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الجموم",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الكامل",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المويه",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ميسان",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "أضم",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العرضيات",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },
                                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بحرة",
                    AreaId = new Guid("28117125-7275-48e3-a2cf-afc7d1644a30"),

                },

                #endregion           
                
                #region المدينة المنورة
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المدينة المنورة",
                    //ChambersOfCommerceCityCode = 203,
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ينبع",
                    //ChambersOfCommerceCityCode = 205,
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العلا",
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المهد",
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الحناكية",
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بدر",
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = new Guid("42c64585-7272-445c-acc4-62e1f18c4fbf"),


                    CityName = "خيبر",
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العيص",
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },
                  new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "وادي الفرع",
                    AreaId = new Guid("6a1afc80-af39-46c9-ae2a-4bf4bbf36040"),

                },

                #endregion

                #region القصيم
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بريدة",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "عنيزة",
                    //ChambersOfCommerceCityCode = 602,
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الرس",
                    //ChambersOfCommerceCityCode = 604,
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المذنب",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "البكيرية",
                    //ChambersOfCommerceCityCode = 603,
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "البدائع",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الأسياح",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "النبهانية",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الشماسية",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "عيون الجواء",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "رياض الخبراء",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "عقلة الصقور",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },
                    new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ضرية",
                    AreaId = new Guid("e8743c10-7a9f-410f-9b67-800e1bafc5d2"),

                },

                #endregion

                #region المنطقة الشرقية
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الدمام",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الأحساء",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "حفر الباطن",
                    //ChambersOfCommerceCityCode = 303,
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الجبيل",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "القطيف",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الخبر",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الخفجي",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "رأس تنورة",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بقيق",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "النعيرية",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "قرية العليا",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العديد",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "البيضاء",
                    AreaId = new Guid("d3fcb090-2a6e-4389-8eb7-a089a4cf4c0e"),

                },

                #endregion

                #region عسير
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "أبها",
                    //ChambersOfCommerceCityCode = 701,
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "خميس مشيط",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بيشة",
                    //ChambersOfCommerceCityCode = 706,
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "النماص",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "محايل عسير",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ظهران الجنوب",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "تثليث",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "سراة عبيدة",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "رجال ألمع",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بلقرن",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "أحد رفيدة",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المجاردة",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "البرك",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بارق",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "تنومة",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "طريب",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الحرجة",
                    AreaId = new Guid("0a692dce-3b9a-45eb-9d47-1c4cbfba0761"),

                },

                #endregion

                #region تبوك
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "تبوك",
                    //ChambersOfCommerceCityCode = 401,
                    AreaId = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الوجه",
                    AreaId = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ضباء",
                    AreaId = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "تيماء",
                    AreaId = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "أملج",
                    AreaId = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "حقل",
                    AreaId = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "البدع",
                    AreaId = new Guid("ed626869-b920-4cfd-81fb-616544b42674"),

                },

                #endregion

                #region حائل
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "حائل",
                    //ChambersOfCommerceCityCode = 402,
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بقعاء",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الغزالة",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الشنان",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الحائط",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "السليمي",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الشملي",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "موقق",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "سميراء",
                    AreaId = new Guid("52101b9b-ec02-4474-a42a-a1f1fe0f881c"),

                },

                #endregion

                #region الحدود الشمالية
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "عرعر",
                    //ChambersOfCommerceCityCode = 403,
                    AreaId = new Guid("86efb2d9-4b25-4e46-b13d-4ec1cf8048d7"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "رفحاء",
                    AreaId = new Guid("86efb2d9-4b25-4e46-b13d-4ec1cf8048d7"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "طريف",
                    AreaId = new Guid("86efb2d9-4b25-4e46-b13d-4ec1cf8048d7"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العويقيلة",
                    AreaId = new Guid("86efb2d9-4b25-4e46-b13d-4ec1cf8048d7"),

                },

                #endregion

                #region جازان
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "جازان",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "صبيا",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "أبو عريش",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "صامطة",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بيش",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الدرب",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الحرث",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ضمد",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الريث",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "فرسان",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الدائر",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العارضة",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "أحد المسارحة",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العيدابي",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "فيفاء",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الطوال",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "هروب",
                    AreaId = new Guid("5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc"),

                },

                #endregion

                #region نجران
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "نجران",
                    //ChambersOfCommerceCityCode = 704,
                    AreaId = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "شرورة",
                    AreaId = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "حبونا",
                    AreaId = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بدر الجنوب",
                    AreaId = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "يدمة",
                    AreaId = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "ثار",
                    AreaId = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "خباش",
                    AreaId = new Guid("1f93fa8f-7d53-48ac-b88a-181b5e406a7b"),

                },

                #endregion

                #region الباحة
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الباحة",
                    //ChambersOfCommerceCityCode = 703,
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بلجرشي",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المندق",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "المخواة",
                    //ChambersOfCommerceCityCode = 705,
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "قلوة",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "العقيق",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "القرى",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "غامد الزناد",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "الحجرة",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },
                 new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "بني حسن",
                    AreaId = new Guid("d6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79"),

                },

                #endregion

                #region الجوف
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "سكاكا",
                    AreaId = new Guid("7355b4f0-9271-46ca-88bd-3c0eb8bef67a"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "القريات",
                    //ChambersOfCommerceCityCode = 405,
                    AreaId = new Guid("7355b4f0-9271-46ca-88bd-3c0eb8bef67a"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "دومة الجندل",
                    AreaId = new Guid("7355b4f0-9271-46ca-88bd-3c0eb8bef67a"),

                },
                new SysCity
                {
                    Id = Guid.NewGuid(),


                    CityName = "طبرجل",
                    AreaId = new Guid("7355b4f0-9271-46ca-88bd-3c0eb8bef67a"),

                },

                #endregion
            }
            );
            context.SaveChanges();
        }
    }
}