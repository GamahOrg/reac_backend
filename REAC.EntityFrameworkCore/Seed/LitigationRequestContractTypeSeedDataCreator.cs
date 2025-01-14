using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class LitigationRequestContractTypeSeedDataCreator
{
    /// <summary>
    /// إيجار – أملاك-وافى
    /// </summary>
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysLitigationRequestContractTypes.AnyAsync()) return;
        await context.SysLitigationRequestContractTypes.AddRangeAsync(
            new List<SysLitigationRequestContractType>
            {
                //RealEstateMediation
                    new SysLitigationRequestContractType
                    {
                        Id = Domain.Shared.Enum.LitigationRequestContractType.Rent,
                        ReconciliationPeriod = 30,
                        CodingPrefixSymbol = "R",
                        TypeName = "إيجار",
                        AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                       TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                        Translations=new List<SysLitigationRequestContractTypeTranslation>
                        {
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "Rent",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "إيجار",
                                LanguageCode = Constants.ArabicLanguage ,
                                AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                       TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",

                            }
                        }
                    },
                    new SysLitigationRequestContractType
                    {
                        Id = Domain.Shared.Enum.LitigationRequestContractType.Properties,
                        ReconciliationPeriod = 30,
                        TypeName = "ملاك",
                        CodingPrefixSymbol = "O",
                        AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                       TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                        Translations=new List<SysLitigationRequestContractTypeTranslation>
                        {
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "Properties",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "ملاك",
                                LanguageCode = Constants.ArabicLanguage,
                                AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                       TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                            }
                        }
                    },
                    new SysLitigationRequestContractType
                    {
                        Id = Domain.Shared.Enum.LitigationRequestContractType.Wafi,
                        ReconciliationPeriod = 30,
                        TypeName = "وافى",
                        AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                        TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                        CodingPrefixSymbol = "W",
                        Translations=new List<SysLitigationRequestContractTypeTranslation>
                        {
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "Wafi",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "وافى",
                                LanguageCode = Constants.ArabicLanguage,
                                AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                       TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                            }
                        }
                    },
                    new SysLitigationRequestContractType
                    {
                        Id = Domain.Shared.Enum.LitigationRequestContractType.RealEstateBrokerage,
                        ReconciliationPeriod = 30,
                        TypeName = "وساطه عقارية",
                        AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                        TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                        CodingPrefixSymbol = "B",
                        Translations=new List<SysLitigationRequestContractTypeTranslation>
                        {
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "Real Estate Brokerage",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "وساطه عقارية",
                                LanguageCode = Constants.ArabicLanguage,
                                AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                                TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                            }
                        }
                    },
                    new SysLitigationRequestContractType
                    {
                        Id = Domain.Shared.Enum.LitigationRequestContractType.OtherServices,
                        TypeName = "خدمات أخرى",
                        ReconciliationPeriod = 30,
                        AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                        TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                        CodingPrefixSymbol = "N",
                        Translations=new List<SysLitigationRequestContractTypeTranslation>
                        {
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "Other Services",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestContractTypeTranslation
                            {
                                TypeName = "خدمات أخرى",
                                LanguageCode = Constants.ArabicLanguage,
                                AcknowledgmentContent = "(أقر أنا {اسم المدعي} هوية رقم {رقم هوية المدعي} مصدرها {مصدر الهوية} بتاريخ {تاريخ الميلادي لليوم} وبكامل أهليتي الشرعية وذلك بعدم وجود دعوى سابقة أو قائمة ضدي من المدعى عليه في ذات موضوع النزاع لدى أي جهة أخرى وهذا إقرار مني بذلك وفي حالة اتضح خلاف ما أقر عليه يحق للمركز اتخاذ الإجراء القانوني ضدي)",
                       TermsAndConditions = " نحتفظ بحقنا بتغيير الشروط والأحكام من وقت لآخر وعليه يتعين عليك الاطلاع على هذه الشروط والأحكام دوريا. ولا يتوجب علينا الاتصال بك أو إعلامك بأي تغييرات تم إجراؤها على الشروط والأحكام. ويعتبر استمرار استخدامك لموقع سكاي نيوز عربية بمثابة موافقة على الشروط والأحكام المطبقة في وقت استخدامك للموقع.",
                            }
                        }
                    }
            });
        context.SaveChanges();
    }
}
