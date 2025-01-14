using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class EmployeeSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysDepartments.AnyAsync()) return;

            await context.SysDepartments.AddRangeAsync(
                new List<SysDepartment>
                {
                    new SysDepartment
                    {
                        Id = Guid.Parse("657004f4-57e1-4715-addc-0e5ff0b00938"),

                        DepartmentName = "إدراة التدقيق",
                        DepartmentType = DepartmentType.Review,
                        DistributionMode = DepartmentDistributionMode.AutoDistribution,
                        Translations=new List<SysDepartmentTranslation>
                        {
                            new SysDepartmentTranslation
                            {
                                DepartmentName="إدراة التدقيق",
                                LanguageCode=Constants.EnglishLanguage
                            },
                            new SysDepartmentTranslation
                            {
                                DepartmentName="Conciliation Department",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        },
                        SysEmployees = new List<SysEmployee>(){
                            new SysEmployee()
                            {
                                 Id = Guid.Parse("17840ab6-b134-4c5f-9273-5c97345010a3"),
                                 CreatedOn = DateTime.Now,
                                 Email = "Con@admin.com",
                                 EmployeeName = "أحمد بن سالم بن محمد",
                                 IsActive = true,
                                 IsLocked = false,
                                 IsDeleted = false,
                                 AllowAssign = true,
                                 AllowReAssign= true,
                                 IsAdmin = true,
                                 Mobile = "0558599225",
                                 UserId = new Guid("17840AB6-B134-4C5F-9273-5C97345010A3"),
                                 Translations = new List<SysEmployeeTranslation>()
                                 {
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "Conciliation Department admin",
                                         LanguageCode=Constants.EnglishLanguage
                                     },
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "أحمد بن سالم بن محمد",
                                         LanguageCode=Constants.ArabicLanguage
                                     }
                                 }
                            },
                            new SysEmployee()
                            {
                                 Id = Guid.Parse("e4c57d8c-86db-49e6-8f7c-a9948a6f860e"),
                                 CreatedOn = DateTime.Now,
                                 Email = "Con1@Employee.com",
                                 EmployeeName = "ريان فواز عسيري",
                                 IsActive = true,
                                 IsLocked = false,
                                 IsDeleted = false,
                                 AllowAssign = true,
                                 AllowReAssign= true,
                                 IsAdmin = false,
                                 Mobile = "0558800225",
                                 UserId = new Guid("E4C57D8C-86DB-49E6-8F7C-A9948A6F860E"),
                                 Translations = new List<SysEmployeeTranslation>()
                                 {
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "Conciliation Department Employee 1",
                                         LanguageCode=Constants.EnglishLanguage
                                     },
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "ريان فواز عسيري",
                                         LanguageCode=Constants.ArabicLanguage
                                     }
                                 }
                            }
                            //,new SysEmployee()
                            //{
                            //     Id = Guid.Parse("ddc90d42-62b3-4a07-b20a-68b0365aec45"),
                            //     CreatedOn = DateTime.Now,
                            //     Email = "Con2@Employee.com",
                            //     EmployeeName = "موظف تدقيق 2",
                            //     IsActive = true,
                            //     IsLocked = false,
                            //     IsDeleted = false,
                            //     AllowAssign = true,
                            //     AllowReAssign= true,
                            //     IsAdmin = false,
                            //     Mobile = "0558800225",
                            //     UserId = new Guid("f83c927d-a5ee-45ca-b95b-be75d09c1e51"),
                            //     Translations = new List<SysEmployeeTranslation>()
                            //     {
                            //         new SysEmployeeTranslation()
                            //         {
                            //             EmployeeName = "Conciliation Department Employee 2",
                            //             LanguageCode=Constants.EnglishLanguage
                            //         },
                            //         new SysEmployeeTranslation()
                            //         {
                            //             EmployeeName = "موظف تدقيق 2",
                            //             LanguageCode=Constants.ArabicLanguage
                            //         }
                            //     }
                            //}
                        }
                    },
                    new SysDepartment
                    {
                        Id = Guid.Parse("fa69b94c-53f2-49ca-8384-072ea272fa08"),
                        DepartmentName = "إدراة الصلح",
                        DepartmentType = DepartmentType.Reconciliation,
                        DistributionMode = DepartmentDistributionMode.AutoDistribution,
                        Translations=new List<SysDepartmentTranslation>
                        {
                            new SysDepartmentTranslation
                            {
                                DepartmentName="إدراة الصلح",
                                LanguageCode=Constants.EnglishLanguage
                            },
                            new SysDepartmentTranslation
                            {
                                DepartmentName="Conciliation Department",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        },
                        SysEmployees = new List<SysEmployee>(){
                            new SysEmployee()
                            {
                                 Id = Guid.Parse("d4430015-c883-413d-b57a-29b5fc6c07c8"),
                                 CreatedOn = DateTime.Now,
                                 Email = "Con@admin.com",
                                 EmployeeName = "أسماء صالح محمد الخليفه ",
                                 IsActive = true,
                                 IsLocked = false,
                                 IsDeleted = false,
                                 AllowAssign = true,
                                 AllowReAssign= true,
                                 IsAdmin = true,
                                 Mobile = "0558500225",
                                 UserId = new Guid("a0c57bde-d7bf-49ca-9662-9665ef3fc835"),
                                 Translations = new List<SysEmployeeTranslation>()
                                 {
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "Conciliation Department admin",
                                         LanguageCode=Constants.EnglishLanguage
                                     },
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "أسماء صالح محمد الخليفه ",
                                         LanguageCode=Constants.ArabicLanguage
                                     }
                                 }
                            },
                            new SysEmployee()
                            {
                                 Id = Guid.Parse("7f14c6d6-8b17-4749-9c60-a07e191bc4f1"),
                                 CreatedOn = DateTime.Now,
                                 Email = "Con1@Employee.com",
                                 EmployeeName = "سعد بسام سعد المالكي",
                                 IsActive = true,
                                 IsLocked = false,
                                 IsDeleted = false,
                                 IsAdmin = false,
                                 AllowAssign = true,
                                 AllowReAssign= true,
                                 Mobile = "0558500225",
                                 UserId = new Guid("b0735375-8b63-4709-93a8-037eead509cf"),
                                 Translations = new List<SysEmployeeTranslation>()
                                 {
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "Conciliation Department Employee 1",
                                         LanguageCode=Constants.EnglishLanguage
                                     },
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "سعد بسام سعد المالكي",
                                         LanguageCode=Constants.ArabicLanguage
                                     }
                                 }
                            },
                        }
                    },
                    new SysDepartment
                    {
                        Id = Guid.Parse("a364bcfe-066a-473f-b852-621c5258945a"),
                        DepartmentName = "إدراة التحكيم",
                        DepartmentType = DepartmentType.Arbitration,
                        DistributionMode = DepartmentDistributionMode.AutoDistribution,
                        Translations=new List<SysDepartmentTranslation>
                        {
                            new SysDepartmentTranslation
                            {
                                DepartmentName="إدراة التحكيم",
                                LanguageCode=Constants.EnglishLanguage
                            },
                            new SysDepartmentTranslation
                            {
                                DepartmentName="Conciliation Department",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        },
                        SysEmployees = new List<SysEmployee>(){
                            new SysEmployee()
                            {
                                 Id = Guid.Parse("301511f5-9025-4235-a724-94e8be2e07a6"),
                                 CreatedOn = DateTime.Now,
                                 Email = "Con@admin.com",
                                 EmployeeName = "فهد منصور ناصر",
                                 IsActive = true,
                                 IsLocked = false,
                                  AllowAssign = true,
                                 AllowReAssign= true,
                                 IsDeleted = false,
                                 IsAdmin = true,
                                 Mobile = "0558500225",
                                 UserId = new Guid("998bb126-eb41-4a6a-a039-e85795c5dcbf"),
                                 Translations = new List<SysEmployeeTranslation>()
                                 {
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "Arbitration Department admin",
                                         LanguageCode=Constants.EnglishLanguage
                                     },
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "فهد منصور ناصر",
                                         LanguageCode=Constants.ArabicLanguage
                                     }
                                 }
                            },
                            new SysEmployee()
                            {
                                 Id = Guid.Parse("8ccace67-f2aa-447d-9cae-054eda040ca4"),
                                 CreatedOn = DateTime.Now,
                                 Email = "Con1@Employee.com",
                                 EmployeeName = "عبدالله بن محمد القرني ",
                                 IsActive = true,
                                 IsLocked = false,
                                 IsDeleted = false,
                                 IsAdmin = false,
                                 Mobile = "0558500225",
                                 UserId = new Guid("4bf8345e-16d7-43ef-b263-935dee667d96"),
                                 Translations = new List<SysEmployeeTranslation>()
                                 {
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "Arbitration Department Employee 1",
                                         LanguageCode=Constants.EnglishLanguage
                                     },
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = "عبدالله بن محمد القرني ",
                                         LanguageCode=Constants.ArabicLanguage
                                     }
                                 }
                            },
                            //new SysEmployee()
                            //{
                            //     Id = Guid.Parse("c969e6af-dca4-4cba-8221-4c8f60c6c67b"),
                            //     CreatedOn = DateTime.Now,
                            //     Email = "Con2@Employee.com",
                            //     EmployeeName = "بدر بن محمد يحي",
                            //     IsActive = true,
                            //     IsLocked = false,
                            //     IsDeleted = false,
                            //     IsAdmin = true,
                            //     Mobile = "0558500225",
                            //     UserId = new Guid("d6adf582-0498-4d52-951e-4d42be7c2502"),
                            //     Translations = new List<SysEmployeeTranslation>()
                            //     {
                            //         new SysEmployeeTranslation()
                            //         {
                            //             EmployeeName = "Arbitration Department Employee 2",
                            //             LanguageCode=Constants.EnglishLanguage
                            //         },
                            //         new SysEmployeeTranslation()
                            //         {
                            //             EmployeeName = "بدر بن محمد يحي",
                            //             LanguageCode=Constants.ArabicLanguage
                            //         }
                            //     }
                            //}
                        }
                    }
                });
            context.SaveChanges();
        }
    }
}
