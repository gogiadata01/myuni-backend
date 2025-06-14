using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyUni.Data;
using MyUni.Models;
using MyUni.Models.Entities;
using System.Xml;
using Newtonsoft.Json;
using MyUni.Controllers;

namespace MyUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnicardEnController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public UnicardEnController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext; 

        }
        [HttpGet]
        public IActionResult GetAllUniCard()
        {
            var AllUniCard = dbContext.MyUniCardEn
                .Select(card => new 
                {
                    Id = card.Id,        
                    Url_en = card.Url_en,    
                    Title_en = card.Title_en, 
                    MainText_en = card.MainText_en 
                })
                .ToList();

            return Ok(AllUniCard);
        }

[HttpPost]
public IActionResult AddUniCardEn([FromBody] UnicardEnDto addUniCardDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

            Console.WriteLine(JsonConvert.SerializeObject(addUniCardDto, Newtonsoft.Json.Formatting.Indented));

    var uniCardEntity = new UnicardEn
    {
        Url_en = addUniCardDto.url_en,
        Title_en = addUniCardDto.title_en,
        MainText_en = addUniCardDto.mainText_en,
        History_en = addUniCardDto.history_en,
        ForPupil_en = addUniCardDto.forPupil_en,
        ScholarshipAndFunding_en = addUniCardDto.scholarshipAndFunding_en,
        ExchangePrograms_en = addUniCardDto.exchangePrograms_en,
        Labs_en = addUniCardDto.labs_en,
        StudentsLife_en = addUniCardDto.studentsLife_en,
        PaymentMethods_en = addUniCardDto.paymentMethods_en,
        Events_en = addUniCardDto.events_en?.Select(e => new UnicardEn.Event_en
        {
            Url_en = e.url_en,
            Title_en = e.title_en,
            Text_en = e.text_en,
            Time_en = e.time_en,
            Link_en = e.link_en
        }).ToList(),
        Sections_en = addUniCardDto.sections_en?.Select(s => new UnicardEn.Section_en
        {
            Title_en = s.title_en,
            ProgramNames_en = s.programNames_en?.Select(p => new UnicardEn.Programname_en
            {
                ProgramName_en = p.programName_en,
                Jobs_en = p.jobs_en,
                SwavlebisEna_en = p.swavlebisEna_en,
                Kvalifikacia_en = p.kvalifikacia_en,
                Dafinanseba_en = p.dafinanseba_en,
                KreditebisRaodenoba_en = p.kreditebisRaodenoba_en,
                AdgilebisRaodenoba_en = p.adgilebisRaodenoba_en,
                Fasi_en = p.fasi_en,
                Kodi_en = p.kodi_en,
                ProgramisAgwera_en = p.programisAgwera_en
            }).ToList()
        }).ToList(),
        Sections2_en = addUniCardDto.sections2_en?.Select(s2 => new UnicardEn.Section2_en
        {
            Title_en = s2.title_en,
            SavaldebuloSagnebi_en = s2.savaldebuloSagnebi_en?.Select(ss => new UnicardEn.SavaldebuloSagnebi_en
            {
                SagnisSaxeli_en = ss.sagnisSaxeli_en,
                Koeficienti_en = ss.koeficienti_en,
                MinimaluriZgvari_en = ss.minimaluriZgvari_en,
                Prioriteti_en = ss.prioriteti_en,
                AdgilebisRaodenoba_en = ss.adgilebisRaodenoba_en
            }).ToList()
        }).ToList(),
        ArchevitiSavaldebuloSaganebi_en = addUniCardDto.archevitiSavaldebuloSaganebi_en?.Select(a => new UnicardEn.ArchevitiSavaldebuloSagani_en
        {
            Title_en = a.title_en,
            ArchevitiSavaldebuloSagnebi_en = a.archevitiSavaldebuloSagnebi_en?.Select(asb => new UnicardEn.ArchevitiSavaldebuloSagnebi_en
            {
                SagnisSaxeli_en = asb.sagnisSaxeli_en,
                Koeficienti_en = asb.koeficienti_en,
                MinimaluriZgvari_en = asb.minimaluriZgvari_en,
                Prioriteti_en = asb.prioriteti_en,
                AdgilebisRaodenoba_en = asb.adgilebisRaodenoba_en
            }).ToList()
        }).ToList()
    };

    dbContext.MyUniCardEn.Add(uniCardEntity);
    dbContext.SaveChanges();

    return Ok(uniCardEntity);
}

    }
    
    
}





        //   [HttpPost]
        // public IActionResult AddUniCard([FromBody] UnicardEnDto addUniCardDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     // Log received data
        //     Console.WriteLine(JsonConvert.SerializeObject(addUniCardDto, Newtonsoft.Json.Formatting.Indented));

        //     // Existing code to map and save entity...
        //     var UniCardEntity = new UnicardEnDto
        //     {
        //         Url_en = addUniCardDto.Url_en,
        //         Title_en = addUniCardDto.Title_en,
        //         MainText_en = addUniCardDto.MainText_en,
        //         History_en = addUniCardDto.History_en,
        //         ForPupil_en = addUniCardDto.ForPupil_en,
        //         ScholarshipAndFunding_en = addUniCardDto.ScholarshipAndFunding_en,
        //         ExchangePrograms_en = addUniCardDto.ExchangePrograms_en,
        //         Labs_en = addUniCardDto.Labs_en,
        //         StudentsLife_en = addUniCardDto.StudentsLife_en,
        //         PaymentMethods_en = addUniCardDto.PaymentMethods_en,
        //         Events_en = addUniCardDto.Events_en?.Select(e => new UnicardEnDto.Event_en
        //         {
        //             Url_en = e.Url_en,
        //             Title_en = e.Title_en,
        //             Text_en = e.Text_en
        //         }).ToList(),
        //         Sections_en = addUniCardDto.Sections_en?.Select(s => new UnicardEnDto.Section_en
        //         {
        //             Title_en = s.Title_en,
        //             ProgramNames_en = s.ProgramNames_en?.Select(p => new UnicardEnDto.Programname_en
        //             {
        //                 ProgramName_en = p.ProgramName_en,
        //                 Jobs_en = p.Jobs_en,
        //                 SwavlebisEna_en = p.SwavlebisEna_en,
        //                 Kvalifikacia_en = p.Kvalifikacia_en,
        //                 Dafinanseba_en = p.Dafinanseba_en,
        //                 KreditebisRaodenoba_en = p.KreditebisRaodenoba_en,
        //                 AdgilebisRaodenoba_en = p.AdgilebisRaodenoba_en,
        //                 Fasi_en = p.Fasi_en,
        //                 Kodi_en = p.Kodi_en,
        //                 ProgramisAgwera_en = p.ProgramisAgwera_en,
        //             }).ToList()
        //         }).ToList(),
        //         Sections2_en = addUniCardDto.Sections2_en?.Select(s2 => new UnicardEnDto.Section2_en
        //         {
        //             Title_en = s2.Title_en,
        //             SavaldebuloSagnebi_en = s2.SavaldebuloSagnebi_en?.Select(ss => new UnicardEnDto.SavaldebuloSagnebi_en
        //             {
        //                 SagnisSaxeli_en = ss.SagnisSaxeli_en,
        //                 Koeficienti_en = ss.Koeficienti_en,
        //                 MinimaluriZgvari_en = ss.MinimaluriZgvari_en,
        //                 Prioriteti_en = ss.Prioriteti_en,
        //                 AdgilebisRaodenoba_en = ss.AdgilebisRaodenoba_en,
        //             }).ToList()
        //         }).ToList(),
        //         ArchevitiSavaldebuloSaganebi_en = addUniCardDto.ArchevitiSavaldebuloSaganebi_en?.Select(a => new UnicardEnDto.ArchevitiSavaldebuloSagani_en
        //         {
        //             Title_en = a.Title_en,
        //             ArchevitiSavaldebuloSagnebi_en = a.ArchevitiSavaldebuloSagnebi_en?.Select(asb => new UnicardEnDto.ArchevitiSavaldebuloSagnebi_en
        //             {
        //                 SagnisSaxeli_en = asb.SagnisSaxeli_en,
        //                 Koeficienti_en = asb.Koeficienti_en,
        //                 MinimaluriZgvari_en = asb.MinimaluriZgvari_en,
        //                 Prioriteti_en = asb.Prioriteti_en,
        //                 AdgilebisRaodenoba_en = asb.AdgilebisRaodenoba_en
        //             }).ToList()
        //         }).ToList()
        //     };

        //     dbContext.MyUniCardEn.Add(UniCardEntity);
        //     dbContext.SaveChanges();

        //     return Ok(UniCardEntity);
        // }