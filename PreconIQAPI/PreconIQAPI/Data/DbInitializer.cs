using PreconIQAPI.Models;
using System.ComponentModel.Design;
using System.Numerics;

namespace PreconIQAPI.Data
{
    public class DbInitializer
    {
        /// <summary>
        /// Method to seed the database with sample data if there is no data initially in the tables.
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(PreconDbContext context)
        {
            _ = context.Database.EnsureCreated();

            if (!context.Projects.Any())
            {
                var project = new List<Project>()
                {
                    new Project
                    {
                        OpportunityId = "0063l00000iwWGxAAM",
                        OpportunityName = "Northern Colorado Repower",
                        OpportunityLegalContractProjectName = "Northern Colorado Repower",
                        OpportunityStageName = "Closed - Archived",
                        ProjectNumber = null,
                        AccountName = "NextEra Energy Constructors, LLC",
                        IsWon = false,
                        IsClosed = true,
                        IsActive = false,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "0063l00000iyiyfAAA",
                        OpportunityName = "Pioneer Creek Wind",
                        OpportunityLegalContractProjectName = "Pioneer Creek Wind",
                        OpportunityStageName = "Bid Submitted",
                        ProjectNumber = null,
                        AccountName = "NextEra Energy Constructors, LLC",
                        IsWon = false,
                        IsClosed = false,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "0063l00000iyMRRAA2",
                        OpportunityName = "Rock Creek Wind I",
                        OpportunityLegalContractProjectName = "Rock Creek Wind I",
                        OpportunityStageName = "Under Warranty",
                        ProjectNumber = null,
                        AccountName = "Invenergy, LLC",
                        IsWon = false,
                        IsClosed = false,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "0063l00000iyowsAAA",
                        OpportunityName = "Maverick 6 & 7 Solar",
                        OpportunityLegalContractProjectName = "Maverick 6 & 7 Solar",
                        OpportunityStageName = "Closed - Complete",
                        ProjectNumber = null,
                        AccountName = "EDF Renewables, Inc",
                        IsWon = true,
                        IsClosed = true,
                        IsActive = false,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "0063l00000kIsAmAAK",
                        OpportunityName = "Koshkonong Solar",
                        OpportunityLegalContractProjectName = "Koshkonong Solar",
                        OpportunityStageName = "In Construction",
                        ProjectNumber = null,
                        AccountName = "Invenergy, LLC",
                        IsWon = false,
                        IsClosed = false,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "0063l00000kJsktAAC",
                        OpportunityName = "Space City Tranche 2",
                        OpportunityLegalContractProjectName = "Space City Tranche 2",
                        OpportunityStageName = "Closed - Project On Hold",
                        ProjectNumber = null,
                        AccountName = "EDF Renewables, Inc",
                        IsWon = false,
                        IsClosed = true,
                        IsActive = false,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "0063l00000kJUyQAAW",
                        OpportunityName = "Sun Valley Solar",
                        OpportunityLegalContractProjectName = "Sun Valley Solar",
                        OpportunityStageName = "Closed - Complete",
                        ProjectNumber = null,
                        AccountName = "ENGIE Renewables NA LLC",
                        IsWon = true,
                        IsClosed = true,
                        IsActive = false,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "0063l00000s4VTMAA2",
                        OpportunityName = "Lower Snake River Wind II",
                        OpportunityLegalContractProjectName = "Lower Snake River Wind II",
                        OpportunityStageName = "LNTP / Work Orders",
                        ProjectNumber = 1128,
                        AccountName = "Clearway Energy Group, LLC",
                        IsWon = false,
                        IsClosed = false,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new Project
                    {
                        OpportunityId = "006Rd000008xULhIAM",
                        OpportunityName = "Buffalo Gap Repower",
                        OpportunityLegalContractProjectName = "Buffalo Gap Repower",
                        OpportunityStageName = "In Construction",
                        ProjectNumber = 1146,
                        AccountName = "AES Clean Energy",
                        IsWon = false,
                        IsClosed = false,
                        IsActive = true,
                        IsDeleted = false
                    }
                };

                context.Projects.AddRange(project);
                _ = context.SaveChanges();

                var projectMetaData = new List<ProjectMetaData>()
                {
                    new ProjectMetaData
                    {
                        ProjectId = 1,
                        Address = "34001 County Road 78, Peetz, CO 80747",
                        City = "Peetz",
                        State = "CO",
                        ZipCode = null,
                        County = "Logan",
                        BidNumber = "19W043",
                        ContractType = "Repower Agreement",
                        Duration = 10,
                        MarketSegment = "Repower Wind",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null,
                        EstimatedFinalContractRevenue = 15167944.67m,
                        MarginPercent = 17.37m,
                        MarginDollars = null,
                        Megawatts = 174.30m,
                        RevenueType = "Awarded",
                        Revenue = 15167945.00m,
                        ProjectStartDate = new DateOnly(2020, 3, 2), //todo check this value
                        ProjectCompletionDate = new DateOnly(2020, 12, 2), //todo check this value
                        CloseDate = new DateOnly(2022, 1,22),
                        SubstantialCompletionDate = new DateOnly(2020, 12, 2), //todo check this value
                        Latitude = 40.994430m,
                        Longitude = -102.910430m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 2,
                        Address = null,
                        City = "Dodge City",
                        State = "KS",
                        ZipCode = null,
                        County = "Ford",
                        BidNumber = "25W071",
                        ContractType = null,
                        Duration = 8,
                        MarketSegment = "Wind",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null,
                        EstimatedFinalContractRevenue = null,
                        MarginPercent = 14.24m,
                        MarginDollars = null,
                        Megawatts = 253.80m,
                        RevenueType = "Targeted",
                        Revenue = 86972530.00m,
                        ProjectStartDate = new DateOnly(2026, 5, 18),
                        ProjectCompletionDate = new DateOnly(2027, 1, 15),
                        CloseDate = new DateOnly(2026, 4, 15),
                        SubstantialCompletionDate = new DateOnly(2026, 11, 25),
                        Latitude = 37.756930m,
                        Longitude = -100.018880m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 3,
                        Address = "60 County Rd 15, Mcfadden, WY 82083",
                        City = "McFadden",
                        State = "WY",
                        ZipCode = null,
                        County = "Albany & Carbon",
                        BidNumber = "22W055",
                        ContractType = "BOP",
                        Duration = 25,
                        MarketSegment = "Wind",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null, //"0063l00000pO2hXAAS",
                        EstimatedFinalContractRevenue = null,
                        MarginPercent = 12.51m,
                        MarginDollars = null,
                        Megawatts = 195.20m,
                        RevenueType = "Awarded",
                        Revenue = 84092452.00m,
                        ProjectStartDate = new DateOnly(2023, 4, 28),
                        ProjectCompletionDate = new DateOnly(2024, 12, 02),
                        CloseDate = new DateOnly(2022, 8, 26),
                        SubstantialCompletionDate = new DateOnly(2025, 4, 28),
                        Latitude = 41.657360m,
                        Longitude = -106.097330m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 4,
                        Address = "29722 Corn Springs Road Desert Center, CA 92239",
                        City = "Desert Center",
                        State = "CA",
                        ZipCode = null,
                        County = "Riverside",
                        BidNumber = "20S007, 20S008",
                        ContractType = null,
                        Duration = 13,
                        MarketSegment = "Solar",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null, //"0063l00000noEugAAE",
                        EstimatedFinalContractRevenue = 177128142.97m,
                        MarginPercent = 9.30m,
                        MarginDollars = null,
                        Megawatts = 310.50m,
                        RevenueType = "Awarded",
                        Revenue = 175480372.00m,
                        ProjectStartDate = new DateOnly(2020, 9, 16),
                        ProjectCompletionDate = new DateOnly(2022, 2, 25),
                        CloseDate = new DateOnly(2020, 6, 5),
                        SubstantialCompletionDate = new DateOnly(2021, 9, 22),
                        Latitude = 33.622180m,
                        Longitude = -115.299230m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 5,
                        Address = null,
                        City = "Cambridge",
                        State = "WI",
                        ZipCode = null,
                        County = "Dane",
                        BidNumber = "23S109C",
                        ContractType = "BOS",
                        Duration = 22,
                        MarketSegment = "Solar",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null, //"0063l00000tMWXvAAO",
                        EstimatedFinalContractRevenue = null,
                        MarginPercent = 15.00m,
                        MarginDollars = null,
                        Megawatts = 387.00m,
                        RevenueType = "Awarded",
                        Revenue = 275476603.00m,
                        ProjectStartDate = new DateOnly(2025, 3, 31),
                        ProjectCompletionDate = new DateOnly(2026, 12, 29),
                        CloseDate = new DateOnly(2025, 2, 1),
                        SubstantialCompletionDate = new DateOnly(2026, 12, 29),
                        Latitude = 43.003740m,
                        Longitude = -89.017500m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 6,
                        Address = null,
                        City = null,
                        State = "TX",
                        ZipCode = null,
                        County = "Wharton",
                        BidNumber = "20S020C-1",
                        ContractType = null,
                        Duration = 6,
                        MarketSegment = "Solar",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null,
                        EstimatedFinalContractRevenue = null,
                        MarginPercent = 8.46m,
                        MarginDollars = null,
                        Megawatts = 132.00m,
                        RevenueType = "Awarded",
                        Revenue = 56284800.00m,
                        ProjectStartDate = new DateOnly(2025, 1, 1),
                        ProjectCompletionDate = new DateOnly(2025, 7, 30),
                        CloseDate = new DateOnly(2024, 6, 1),
                        SubstantialCompletionDate = new DateOnly(2025, 6, 30),
                        Latitude = 30.267590m,
                        Longitude = -97.742990m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 7,
                        Address = "884 HCR 3110 S.  Abbott, TX 76621",
                        City = "Abbott",
                        State = "TX",
                        ZipCode = null,
                        County = "Hill",
                        BidNumber = "20S055",
                        ContractType = "BOP",
                        Duration = 22,
                        MarketSegment = "Solar",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null, //"0063l00000nowaWAAQ",
                        EstimatedFinalContractRevenue = 160056715.11m,
                        MarginPercent = 10.60m,
                        MarginDollars = null,
                        Megawatts = 347.77m,
                        RevenueType = "Awarded",
                        Revenue = 157609364.00m,
                        ProjectStartDate = new DateOnly(2021, 3, 1),
                        ProjectCompletionDate = new DateOnly(2022, 12, 31),
                        CloseDate = new DateOnly(2021, 6, 1),
                        SubstantialCompletionDate = new DateOnly(2022, 12, 30),
                        Latitude = 31.865660m,
                        Longitude = -97.014000m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 8,
                        Address = null,
                        City = "Near Pomeroy",
                        State = "WA",
                        ZipCode = null,
                        County = "Garfield and Columbia",
                        BidNumber = "25W008",
                        ContractType = "null",
                        Duration = 9,
                        MarketSegment = "Wind",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null,
                        EstimatedFinalContractRevenue = null,
                        MarginPercent = 13m,
                        MarginDollars = null,
                        Megawatts = 153m,
                        RevenueType = "Awarded",
                        Revenue = 140046564m,
                        ProjectStartDate = new DateOnly(2027, 3, 22), //todo check this value
                        ProjectCompletionDate = new DateOnly(2027, 12, 10), //todo check this value
                        CloseDate = new DateOnly(2025, 11, 3),
                        SubstantialCompletionDate = new DateOnly(2027, 11, 22), //todo check this value
                        Latitude = 46.47357m,
                        Longitude = -117.60089m
                    },
                    new ProjectMetaData
                    {
                        ProjectId = 9,
                        Address = null,
                        City = "Abilene",
                        State = "TX",
                        ZipCode = null,
                        County = "Nolan & Taylor",
                        BidNumber = "24W052B_CON2",
                        ContractType = "Repower Agreement",
                        Duration = 16,
                        MarketSegment = "Repower Wind",
                        OpportunityDescription = null,
                        BidAsCompany = null,
                        IsParentOpportunity = false,
                        ParentProjectId = null,
                        EstimatedFinalContractRevenue = 285724548.25m,
                        MarginPercent = 15.14m,
                        MarginDollars = null,
                        Megawatts = 526.5m,
                        RevenueType = "Awarded",
                        Revenue = 288916986m,
                        ProjectStartDate = new DateOnly(2020, 3, 2), //todo check this value
                        ProjectCompletionDate = new DateOnly(2020, 12, 2), //todo check this value
                        CloseDate = new DateOnly(2025, 1, 1),
                        SubstantialCompletionDate = new DateOnly(2026, 12, 30), //todo check this value
                        Latitude = 32.45748m,
                        Longitude = -99.73412m
                    }
                };

                context.ProjectMetaData.AddRange(projectMetaData);
                _ = context.SaveChanges();

                var projectNormals = new List<ProjectNormals>()
                {
                    new ProjectNormals { ProjectId = 1, Month = 1, AvgHighTemp = 40.6m, AvgLowTemp = 15.8m, RecordAvgHighTemp = 52m, RecordAvgLowTemp = 8.3m, AvgRain = 0.4m, RecordHighAvgRain = 2m, RecordLowAvgRain = 0m, AvgSnow = 5.3m, RecordHighAvgSnow = 28m, RecordLowAvgSnow = 0m, AvgHighWind = 14.05m, AvgLowWind = 12.05m, RecordAvgHighWind = 16.05m, RecordAvgLowWind = 10.65m, },
                    new ProjectNormals { ProjectId = 1, Month = 2, AvgHighTemp = 42.3m, AvgLowTemp = 17.2m, RecordAvgHighTemp = 50.9m, RecordAvgLowTemp = 10.4m, AvgRain = 0.5m, RecordHighAvgRain = 1.2m, RecordLowAvgRain = 0m, AvgSnow = 5.9m, RecordHighAvgSnow = 13m, RecordLowAvgSnow = 0m, AvgHighWind = 13.98m, AvgLowWind = 11.98m, RecordAvgHighWind = 15.98m, RecordAvgLowWind = 11.09m, },
                    new ProjectNormals { ProjectId = 1, Month = 3, AvgHighTemp = 52.8m, AvgLowTemp = 25.5m, RecordAvgHighTemp = 65.1m, RecordAvgLowTemp = 19m, AvgRain = 1.1m, RecordHighAvgRain = 4.1m, RecordLowAvgRain = 0m, AvgSnow = 4.3m, RecordHighAvgSnow = 15.9m, RecordLowAvgSnow = 0m, AvgHighWind = 14.93m, AvgLowWind = 12.93m, RecordAvgHighWind = 16.93m, RecordAvgLowWind = 12m, },
                    new ProjectNormals { ProjectId = 1, Month = 4, AvgHighTemp = 60.1m, AvgLowTemp = 32m, RecordAvgHighTemp = 68m, RecordAvgLowTemp = 27.3m, AvgRain = 1.9m, RecordHighAvgRain = 4.1m, RecordLowAvgRain = 0.2m, AvgSnow = 3.2m, RecordHighAvgSnow = 14m, RecordLowAvgSnow = 0m, AvgHighWind = 15.17m, AvgLowWind = 13.17m, RecordAvgHighWind = 17.17m, RecordAvgLowWind = 12.79m, },
                    new ProjectNormals { ProjectId = 1, Month = 5, AvgHighTemp = 69m, AvgLowTemp = 42m, RecordAvgHighTemp = 76.3m, RecordAvgLowTemp = 37.1m, AvgRain = 3.1m, RecordHighAvgRain = 9m, RecordLowAvgRain = 0.6m, AvgSnow = 0.8m, RecordHighAvgSnow = 6m, RecordLowAvgSnow = 0m, AvgHighWind = 16.24m, AvgLowWind = 14.24m, RecordAvgHighWind = 18.24m, RecordAvgLowWind = 12.82m, },
                    new ProjectNormals { ProjectId = 1, Month = 6, AvgHighTemp = 82m, AvgLowTemp = 52.2m, RecordAvgHighTemp = 91.3m, RecordAvgLowTemp = 44.7m, AvgRain = 2.9m, RecordHighAvgRain = 9.9m, RecordLowAvgRain = 1.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 14.02m, AvgLowWind = 12.02m, RecordAvgHighWind = 16.02m, RecordAvgLowWind = 10.93m, },
                    new ProjectNormals { ProjectId = 1, Month = 7, AvgHighTemp = 89.2m, AvgLowTemp = 59m, RecordAvgHighTemp = 94.2m, RecordAvgLowTemp = 54.3m, AvgRain = 2.6m, RecordHighAvgRain = 6.1m, RecordLowAvgRain = 0.6m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 12.28m, AvgLowWind = 10.28m, RecordAvgHighWind = 14.28m, RecordAvgLowWind = 10.11m, },
                    new ProjectNormals { ProjectId = 1, Month = 8, AvgHighTemp = 86.8m, AvgLowTemp = 56.7m, RecordAvgHighTemp = 93.7m, RecordAvgLowTemp = 51.5m, AvgRain = 2.1m, RecordHighAvgRain = 5.3m, RecordLowAvgRain = 0.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.62m, AvgLowWind = 9.62m, RecordAvgHighWind = 13.62m, RecordAvgLowWind = 9.3m, },
                    new ProjectNormals { ProjectId = 1, Month = 9, AvgHighTemp = 79m, AvgLowTemp = 48.1m, RecordAvgHighTemp = 87.7m, RecordAvgLowTemp = 42.4m, AvgRain = 1.6m, RecordHighAvgRain = 8.4m, RecordLowAvgRain = 0.1m, AvgSnow = 0.1m, RecordHighAvgSnow = 2m, RecordLowAvgSnow = 0m, AvgHighWind = 12.33m, AvgLowWind = 10.33m, RecordAvgHighWind = 14.33m, RecordAvgLowWind = 9.18m, },
                    new ProjectNormals { ProjectId = 1, Month = 10, AvgHighTemp = 63.9m, AvgLowTemp = 34.8m, RecordAvgHighTemp = 74.9m, RecordAvgLowTemp = 26.7m, AvgRain = 1m, RecordHighAvgRain = 3m, RecordLowAvgRain = 0m, AvgSnow = 2.6m, RecordHighAvgSnow = 15m, RecordLowAvgSnow = 0m, AvgHighWind = 12.97m, AvgLowWind = 10.97m, RecordAvgHighWind = 14.97m, RecordAvgLowWind = 10.93m, },
                    new ProjectNormals { ProjectId = 1, Month = 11, AvgHighTemp = 51.9m, AvgLowTemp = 24.4m, RecordAvgHighTemp = 6m, RecordAvgLowTemp = 15.1m, AvgRain = 0.5m, RecordHighAvgRain = 1.4m, RecordLowAvgRain = 0m, AvgSnow = 3.3m, RecordHighAvgSnow = 1m, RecordLowAvgSnow = 0m, AvgHighWind = 12.97m, AvgLowWind = 10.97m, RecordAvgHighWind = 14.97m, RecordAvgLowWind = 10.48m, },
                    new ProjectNormals { ProjectId = 1, Month = 12, AvgHighTemp = 42.1m, AvgLowTemp = 17.6m, RecordAvgHighTemp = 51.6m, RecordAvgLowTemp = 9.4m, AvgRain = 0.4m, RecordHighAvgRain = 1.4m, RecordLowAvgRain = 0m, AvgSnow = 5.3m, RecordHighAvgSnow = 19.2m, RecordLowAvgSnow = 0m, AvgHighWind = 14.16m, AvgLowWind = 12.16m, RecordAvgHighWind = 16.16m, RecordAvgLowWind = 11.42m, },
                    new ProjectNormals { ProjectId = 2, Month = 1, AvgHighTemp = 45.2m, AvgLowTemp = 20.2m, RecordAvgHighTemp = 58.2m, RecordAvgLowTemp = 14.9m, AvgRain = 0.6m, RecordHighAvgRain = 2.6m, RecordLowAvgRain = 0m, AvgSnow = 4.7m, RecordHighAvgSnow = 14.8m, RecordLowAvgSnow = 0m, AvgHighWind = 13.1m, AvgLowWind = 11.1m, RecordAvgHighWind = 15.1m, RecordAvgLowWind = 10.98m, },
                    new ProjectNormals { ProjectId = 2, Month = 2, AvgHighTemp = 48.6m, AvgLowTemp = 22.3m, RecordAvgHighTemp = 60.3m, RecordAvgLowTemp = 13.5m, AvgRain = 0.5m, RecordHighAvgRain = 1.6m, RecordLowAvgRain = 0m, AvgSnow = 4.4m, RecordHighAvgSnow = 14.3m, RecordLowAvgSnow = 0m, AvgHighWind = 14.55m, AvgLowWind = 12.55m, RecordAvgHighWind = 16.55m, RecordAvgLowWind = 12.12m, },
                    new ProjectNormals { ProjectId = 2, Month = 3, AvgHighTemp = 59.5m, AvgLowTemp = 31.2m, RecordAvgHighTemp = 68.7m, RecordAvgLowTemp = 22.1m, AvgRain = 1.4m, RecordHighAvgRain = 5m, RecordLowAvgRain = 0m, AvgSnow = 3.2m, RecordHighAvgSnow = 19m, RecordLowAvgSnow = 0m, AvgHighWind = 15.97m, AvgLowWind = 13.97m, RecordAvgHighWind = 17.97m, RecordAvgLowWind = 13.29m, },
                    new ProjectNormals { ProjectId = 2, Month = 4, AvgHighTemp = 68.7m, AvgLowTemp = 40.3m, RecordAvgHighTemp = 76.9m, RecordAvgLowTemp = 32.9m, AvgRain = 1.8m, RecordHighAvgRain = 8.1m, RecordLowAvgRain = 0m, AvgSnow = 0.8m, RecordHighAvgSnow = 1m, RecordLowAvgSnow = 0m, AvgHighWind = 16.42m, AvgLowWind = 14.42m, RecordAvgHighWind = 18.42m, RecordAvgLowWind = 14.27m, },
                    new ProjectNormals { ProjectId = 2, Month = 5, AvgHighTemp = 78.1m, AvgLowTemp = 51.2m, RecordAvgHighTemp = 87m, RecordAvgLowTemp = 47.4m, AvgRain = 3m, RecordHighAvgRain = 10.3m, RecordLowAvgRain = 0.2m, AvgSnow = 0m, RecordHighAvgSnow = 1m, RecordLowAvgSnow = 0m, AvgHighWind = 14.08m, AvgLowWind = 12.08m, RecordAvgHighWind = 16.08m, RecordAvgLowWind = 10.97m, },
                    new ProjectNormals { ProjectId = 2, Month = 6, AvgHighTemp = 88.6m, AvgLowTemp = 62.1m, RecordAvgHighTemp = 96.5m, RecordAvgLowTemp = 58.9m, AvgRain = 3.5m, RecordHighAvgRain = 12m, RecordLowAvgRain = 0.3m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 15.68m, AvgLowWind = 13.68m, RecordAvgHighWind = 17.68m, RecordAvgLowWind = 12.93m, },
                    new ProjectNormals { ProjectId = 2, Month = 7, AvgHighTemp = 93.4m, AvgLowTemp = 66.8m, RecordAvgHighTemp = 102.4m, RecordAvgLowTemp = 64m, AvgRain = 3.2m, RecordHighAvgRain = 8.4m, RecordLowAvgRain = 0.5m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 13.0m, AvgLowWind = 11m, RecordAvgHighWind = 15.0m, RecordAvgLowWind = 10.93m, },
                    new ProjectNormals { ProjectId = 2, Month = 8, AvgHighTemp = 91.2m, AvgLowTemp = 64.9m, RecordAvgHighTemp = 99.3m, RecordAvgLowTemp = 60.9m, AvgRain = 2.9m, RecordHighAvgRain = 7.3m, RecordLowAvgRain = 0.3m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 12.34m, AvgLowWind = 10.34m, RecordAvgHighWind = 14.34m, RecordAvgLowWind = 10.28m, },
                    new ProjectNormals { ProjectId = 2, Month = 9, AvgHighTemp = 84m, AvgLowTemp = 56.8m, RecordAvgHighTemp = 90.8m, RecordAvgLowTemp = 51.4m, AvgRain = 1.7m, RecordHighAvgRain = 5.1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 1.4m, RecordLowAvgSnow = 0m, AvgHighWind = 12.65m, AvgLowWind = 10.65m, RecordAvgHighWind = 14.65m, RecordAvgLowWind = 9.87m, },
                    new ProjectNormals { ProjectId = 2, Month = 10, AvgHighTemp = 70.9m, AvgLowTemp = 43.2m, RecordAvgHighTemp = 80.2m, RecordAvgLowTemp = 37.2m, AvgRain = 2m, RecordHighAvgRain = 6.5m, RecordLowAvgRain = 0m, AvgSnow = 0.4m, RecordHighAvgSnow = 4.3m, RecordLowAvgSnow = 0m, AvgHighWind = 14.04m, AvgLowWind = 12.04m, RecordAvgHighWind = 16.04m, RecordAvgLowWind = 11.65m, },
                    new ProjectNormals { ProjectId = 2, Month = 11, AvgHighTemp = 57.8m, AvgLowTemp = 30.9m, RecordAvgHighTemp = 66.8m, RecordAvgLowTemp = 24.2m, AvgRain = 0.9m, RecordHighAvgRain = 6.4m, RecordLowAvgRain = 0m, AvgSnow = 1.1m, RecordHighAvgSnow = 8.5m, RecordLowAvgSnow = 0m, AvgHighWind = 13.8m, AvgLowWind = 11.8m, RecordAvgHighWind = 15.8m, RecordAvgLowWind = 11.32m, },
                    new ProjectNormals { ProjectId = 2, Month = 12, AvgHighTemp = 46.4m, AvgLowTemp = 22.1m, RecordAvgHighTemp = 58.4m, RecordAvgLowTemp = 14.7m, AvgRain = 0.9m, RecordHighAvgRain = 4.3m, RecordLowAvgRain = 0m, AvgSnow = 3.8m, RecordHighAvgSnow = 14.9m, RecordLowAvgSnow = 0m, AvgHighWind = 12.4m, AvgLowWind = 10.4m, RecordAvgHighWind = 14.4m, RecordAvgLowWind = 10.01m, },
                    new ProjectNormals { ProjectId = 3, Month = 1, AvgHighTemp = 32.7m, AvgLowTemp = 12.3m, RecordAvgHighTemp = 39.1m, RecordAvgLowTemp = 6.1m, AvgRain = 0.4m, RecordHighAvgRain = 1.2m, RecordLowAvgRain = 0.1m, AvgSnow = 5.9m, RecordHighAvgSnow = 15.2m, RecordLowAvgSnow = 1.3m, AvgHighWind = 12.77m, AvgLowWind = 10.77m, RecordAvgHighWind = 14.77m, RecordAvgLowWind = 9.38m, },
                    new ProjectNormals { ProjectId = 3, Month = 2, AvgHighTemp = 34.3m, AvgLowTemp = 14.1m, RecordAvgHighTemp = 41.1m, RecordAvgLowTemp = 7.1m, AvgRain = 0.5m, RecordHighAvgRain = 1.4m, RecordLowAvgRain = 0.2m, AvgSnow = 6.5m, RecordHighAvgSnow = 19.8m, RecordLowAvgSnow = 2.3m, AvgHighWind = 14.94m, AvgLowWind = 12.94m, RecordAvgHighWind = 16.94m, RecordAvgLowWind = 12.1m, },
                    new ProjectNormals { ProjectId = 3, Month = 3, AvgHighTemp = 44m, AvgLowTemp = 22.1m, RecordAvgHighTemp = 51.3m, RecordAvgLowTemp = 15.4m, AvgRain = 0.9m, RecordHighAvgRain = 2.1m, RecordLowAvgRain = 0m, AvgSnow = 10.1m, RecordHighAvgSnow = 28.3m, RecordLowAvgSnow = 0m, AvgHighWind = 14.7m, AvgLowWind = 12.7m, RecordAvgHighWind = 16.7m, RecordAvgLowWind = 11.78m, },
                    new ProjectNormals { ProjectId = 3, Month = 4, AvgHighTemp = 50.8m, AvgLowTemp = 27.4m, RecordAvgHighTemp = 58.8m, RecordAvgLowTemp = 21.7m, AvgRain = 1.4m, RecordHighAvgRain = 3.3m, RecordLowAvgRain = 0.3m, AvgSnow = 12.2m, RecordHighAvgSnow = 28.6m, RecordLowAvgSnow = 2.5m, AvgHighWind = 14.6m, AvgLowWind = 12.6m, RecordAvgHighWind = 16.6m, RecordAvgLowWind = 12.56m, },
                    new ProjectNormals { ProjectId = 3, Month = 5, AvgHighTemp = 59.9m, AvgLowTemp = 36.3m, RecordAvgHighTemp = 65.5m, RecordAvgLowTemp = 32.6m, AvgRain = 1.8m, RecordHighAvgRain = 2.9m, RecordLowAvgRain = 0.4m, AvgSnow = 4.4m, RecordHighAvgSnow = 16m, RecordLowAvgSnow = 0m, AvgHighWind = 14.62m, AvgLowWind = 12.62m, RecordAvgHighWind = 16.62m, RecordAvgLowWind = 11.72m, },
                    new ProjectNormals { ProjectId = 3, Month = 6, AvgHighTemp = 76m, AvgLowTemp = 47.1m, RecordAvgHighTemp = 82.1m, RecordAvgLowTemp = 43.1m, AvgRain = 1.3m, RecordHighAvgRain = 3.4m, RecordLowAvgRain = 0.2m, AvgSnow = 0.7m, RecordHighAvgSnow = 12m, RecordLowAvgSnow = 0m, AvgHighWind = 15.12m, AvgLowWind = 13.12m, RecordAvgHighWind = 17.12m, RecordAvgLowWind = 11.35m, },
                    new ProjectNormals { ProjectId = 3, Month = 7, AvgHighTemp = 81.3m, AvgLowTemp = 52.6m, RecordAvgHighTemp = 83m, RecordAvgLowTemp = 50.4m, AvgRain = 1.9m, RecordHighAvgRain = 3.1m, RecordLowAvgRain = 0.8m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.52m, AvgLowWind = 9.52m, RecordAvgHighWind = 13.52m, RecordAvgLowWind = 8.64m, },
                    new ProjectNormals { ProjectId = 3, Month = 8, AvgHighTemp = 79.9m, AvgLowTemp = 50.8m, RecordAvgHighTemp = 83.9m, RecordAvgLowTemp = 48m, AvgRain = 1.3m, RecordHighAvgRain = 3.3m, RecordLowAvgRain = 0.2m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 12.34m, AvgLowWind = 10.34m, RecordAvgHighWind = 14.34m, RecordAvgLowWind = 10.01m, },
                    new ProjectNormals { ProjectId = 3, Month = 9, AvgHighTemp = 73m, AvgLowTemp = 43.8m, RecordAvgHighTemp = 75.9m, RecordAvgLowTemp = 40.4m, AvgRain = 1.3m, RecordHighAvgRain = 3.5m, RecordLowAvgRain = 0.2m, AvgSnow = 0.5m, RecordHighAvgSnow = 3.8m, RecordLowAvgSnow = 0m, AvgHighWind = 11.45m, AvgLowWind = 9.45m, RecordAvgHighWind = 13.45m, RecordAvgLowWind = 9.2m, },
                    new ProjectNormals { ProjectId = 3, Month = 10, AvgHighTemp = 57.2m, AvgLowTemp = 31.6m, RecordAvgHighTemp = 66.3m, RecordAvgLowTemp = 21.3m, AvgRain = 1.1m, RecordHighAvgRain = 2.5m, RecordLowAvgRain = 0.2m, AvgSnow = 6.3m, RecordHighAvgSnow = 2m, RecordLowAvgSnow = 0m, AvgHighWind = 13.41m, AvgLowWind = 11.41m, RecordAvgHighWind = 15.41m, RecordAvgLowWind = 10.67m, },
                    new ProjectNormals { ProjectId = 3, Month = 11, AvgHighTemp = 44.6m, AvgLowTemp = 22.5m, RecordAvgHighTemp = 50.5m, RecordAvgLowTemp = 15.4m, AvgRain = 0.6m, RecordHighAvgRain = 1.6m, RecordLowAvgRain = 0m, AvgSnow = 7.2m, RecordHighAvgSnow = 23.9m, RecordLowAvgSnow = 1.7m, AvgHighWind = 14.23m, AvgLowWind = 12.23m, RecordAvgHighWind = 16.23m, RecordAvgLowWind = 11.76m, },
                    new ProjectNormals { ProjectId = 3, Month = 12, AvgHighTemp = 35m, AvgLowTemp = 14.4m, RecordAvgHighTemp = 42.5m, RecordAvgLowTemp = 9.6m, AvgRain = 0.5m, RecordHighAvgRain = 0.8m, RecordLowAvgRain = 0.1m, AvgSnow = 6.4m, RecordHighAvgSnow = 11.9m, RecordLowAvgSnow = 0.6m, AvgHighWind = 14.25m, AvgLowWind = 12.25m, RecordAvgHighWind = 16.25m, RecordAvgLowWind = 11.69m, },
                    new ProjectNormals { ProjectId = 4, Month = 1, AvgHighTemp = 66m, AvgLowTemp = 41.2m, RecordAvgHighTemp = 68.6m, RecordAvgLowTemp = 37m, AvgRain = 0.6m, RecordHighAvgRain = 3m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.91m, AvgLowWind = 6.91m, RecordAvgHighWind = 10.91m, RecordAvgLowWind = 5.32m, },
                    new ProjectNormals { ProjectId = 4, Month = 2, AvgHighTemp = 72.1m, AvgLowTemp = 45.2m, RecordAvgHighTemp = 75.5m, RecordAvgLowTemp = 44m, AvgRain = 0.6m, RecordHighAvgRain = 2.2m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.7m, AvgLowWind = 6.7m, RecordAvgHighWind = 10.7m, RecordAvgLowWind = 6.1m, },
                    new ProjectNormals { ProjectId = 4, Month = 3, AvgHighTemp = 81.3m, AvgLowTemp = 51m, RecordAvgHighTemp = 86.6m, RecordAvgLowTemp = 45.7m, AvgRain = 0.3m, RecordHighAvgRain = 1.1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 10.05m, AvgLowWind = 8.05m, RecordAvgHighWind = 12.05m, RecordAvgLowWind = 7.95m, },
                    new ProjectNormals { ProjectId = 4, Month = 4, AvgHighTemp = 85.2m, AvgLowTemp = 54.9m, RecordAvgHighTemp = 85.2m, RecordAvgLowTemp = 53.2m, AvgRain = 0m, RecordHighAvgRain = 0.2m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 10.02m, AvgLowWind = 8.02m, RecordAvgHighWind = 12.02m, RecordAvgLowWind = 7.45m, },
                    new ProjectNormals { ProjectId = 4, Month = 5, AvgHighTemp = 97.6m, AvgLowTemp = 65.4m, RecordAvgHighTemp = 99.7m, RecordAvgLowTemp = 64.1m, AvgRain = 0m, RecordHighAvgRain = 0.1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.03m, AvgLowWind = 9.03m, RecordAvgHighWind = 13.03m, RecordAvgLowWind = 8.87m, },
                    new ProjectNormals { ProjectId = 4, Month = 6, AvgHighTemp = 104.5m, AvgLowTemp = 72.9m, RecordAvgHighTemp = 107.2m, RecordAvgLowTemp = 70.1m, AvgRain = 0m, RecordHighAvgRain = 0m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.6m, AvgLowWind = 9.6m, RecordAvgHighWind = 13.6m, RecordAvgLowWind = 8.89m, },
                    new ProjectNormals { ProjectId = 4, Month = 7, AvgHighTemp = 109.2m, AvgLowTemp = 81.2m, RecordAvgHighTemp = 109.9m, RecordAvgLowTemp = 78.9m, AvgRain = 0.1m, RecordHighAvgRain = 0.3m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 12.28m, AvgLowWind = 10.28m, RecordAvgHighWind = 14.28m, RecordAvgLowWind = 9.84m, },
                    new ProjectNormals { ProjectId = 4, Month = 8, AvgHighTemp = 105.2m, AvgLowTemp = 77.2m, RecordAvgHighTemp = 105.9m, RecordAvgLowTemp = 77m, AvgRain = 0.5m, RecordHighAvgRain = 2.1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.11m, AvgLowWind = 9.11m, RecordAvgHighWind = 13.11m, RecordAvgLowWind = 8.87m, },
                    new ProjectNormals { ProjectId = 4, Month = 9, AvgHighTemp = 100.3m, AvgLowTemp = 68.9m, RecordAvgHighTemp = 103.7m, RecordAvgLowTemp = 66.3m, AvgRain = 0.4m, RecordHighAvgRain = 1.2m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.38m, AvgLowWind = 6.38m, RecordAvgHighWind = 10.38m, RecordAvgLowWind = 6.21m, },
                    new ProjectNormals { ProjectId = 4, Month = 10, AvgHighTemp = 87.5m, AvgLowTemp = 58.9m, RecordAvgHighTemp = 88.2m, RecordAvgLowTemp = 57.4m, AvgRain = 0.3m, RecordHighAvgRain = 0.7m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 7.09m, AvgLowWind = 5.09m, RecordAvgHighWind = 9.09m, RecordAvgLowWind = 4.74m, },
                    new ProjectNormals { ProjectId = 4, Month = 11, AvgHighTemp = 76m, AvgLowTemp = 48.1m, RecordAvgHighTemp = 80.1m, RecordAvgLowTemp = 46.5m, AvgRain = 0.1m, RecordHighAvgRain = 0.3m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.76m, AvgLowWind = 6.76m, RecordAvgHighWind = 10.76m, RecordAvgLowWind = 6.24m, },
                    new ProjectNormals { ProjectId = 4, Month = 12, AvgHighTemp = 65.7m, AvgLowTemp = 40.6m, RecordAvgHighTemp = 68m, RecordAvgLowTemp = 39.2m, AvgRain = 0.6m, RecordHighAvgRain = 1.5m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 6.36m, AvgLowWind = 4.36m, RecordAvgHighWind = 8.36m, RecordAvgLowWind = 3.66m, },
                    new ProjectNormals { ProjectId = 5, Month = 1, AvgHighTemp = 24.8m, AvgLowTemp = 7.6m, RecordAvgHighTemp = 32.8m, RecordAvgLowTemp = -0.1m, AvgRain = 1.6m, RecordHighAvgRain = 3.5m, RecordLowAvgRain = 0.8m, AvgSnow = 14.2m, RecordHighAvgSnow = 19.1m, RecordLowAvgSnow = 6m, AvgHighWind = 9.38m, AvgLowWind = 7.38m, RecordAvgHighWind = 11.38m, RecordAvgLowWind = 7.16m, },
                    new ProjectNormals { ProjectId = 5, Month = 2, AvgHighTemp = 29.5m, AvgLowTemp = 11.9m, RecordAvgHighTemp = 33.5m, RecordAvgLowTemp = 5.1m, AvgRain = 2.2m, RecordHighAvgRain = 4.4m, RecordLowAvgRain = 1.2m, AvgSnow = 14.9m, RecordHighAvgSnow = 26.2m, RecordLowAvgSnow = 8.4m, AvgHighWind = 10.03m, AvgLowWind = 8.03m, RecordAvgHighWind = 12.03m, RecordAvgLowWind = 7.78m, },
                    new ProjectNormals { ProjectId = 5, Month = 3, AvgHighTemp = 40.4m, AvgLowTemp = 22.1m, RecordAvgHighTemp = 41.5m, RecordAvgLowTemp = 20.3m, AvgRain = 2.6m, RecordHighAvgRain = 3.8m, RecordLowAvgRain = 1.5m, AvgSnow = 7.2m, RecordHighAvgSnow = 13.7m, RecordLowAvgSnow = 3m, AvgHighWind = 10.72m, AvgLowWind = 8.72m, RecordAvgHighWind = 12.72m, RecordAvgLowWind = 7.81m, },
                    new ProjectNormals { ProjectId = 5, Month = 4, AvgHighTemp = 60.3m, AvgLowTemp = 36m, RecordAvgHighTemp = 63.1m, RecordAvgLowTemp = 33.7m, AvgRain = 4m, RecordHighAvgRain = 6.2m, RecordLowAvgRain = 1.6m, AvgSnow = 1.3m, RecordHighAvgSnow = 6.5m, RecordLowAvgSnow = 0m, AvgHighWind = 11.07m, AvgLowWind = 9.07m, RecordAvgHighWind = 13.07m, RecordAvgLowWind = 8.7m, },
                    new ProjectNormals { ProjectId = 5, Month = 5, AvgHighTemp = 67.4m, AvgLowTemp = 46.2m, RecordAvgHighTemp = 72.5m, RecordAvgLowTemp = 41.9m, AvgRain = 4.6m, RecordHighAvgRain = 1m, RecordLowAvgRain = 1.8m, AvgSnow = 0m, RecordHighAvgSnow = 0.1m, RecordLowAvgSnow = 0m, AvgHighWind = 9.2m, AvgLowWind = 7.2m, RecordAvgHighWind = 11.2m, RecordAvgLowWind = 7.15m, },
                    new ProjectNormals { ProjectId = 5, Month = 6, AvgHighTemp = 76.6m, AvgLowTemp = 55.2m, RecordAvgHighTemp = 79.3m, RecordAvgLowTemp = 53.4m, AvgRain = 6m, RecordHighAvgRain = 10.4m, RecordLowAvgRain = 4.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.12m, AvgLowWind = 7.12m, RecordAvgHighWind = 11.12m, RecordAvgLowWind = 6.65m, },
                    new ProjectNormals { ProjectId = 5, Month = 7, AvgHighTemp = 8m, AvgLowTemp = 59.4m, RecordAvgHighTemp = 81.4m, RecordAvgLowTemp = 58.1m, AvgRain = 3.6m, RecordHighAvgRain = 5.2m, RecordLowAvgRain = 1.6m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 7.84m, AvgLowWind = 5.84m, RecordAvgHighWind = 9.84m, RecordAvgLowWind = 5.73m, },
                    new ProjectNormals { ProjectId = 5, Month = 8, AvgHighTemp = 79.1m, AvgLowTemp = 58.5m, RecordAvgHighTemp = 81.4m, RecordAvgLowTemp = 56m, AvgRain = 5.4m, RecordHighAvgRain = 14.2m, RecordLowAvgRain = 1.2m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 7.36m, AvgLowWind = 5.36m, RecordAvgHighWind = 9.36m, RecordAvgLowWind = 4.93m, },
                    new ProjectNormals { ProjectId = 5, Month = 9, AvgHighTemp = 73.5m, AvgLowTemp = 50.2m, RecordAvgHighTemp = 78m, RecordAvgLowTemp = 48.2m, AvgRain = 2.4m, RecordHighAvgRain = 4.1m, RecordLowAvgRain = 1.2m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 6.63m, AvgLowWind = 4.63m, RecordAvgHighWind = 8.63m, RecordAvgLowWind = 4.4m, },
                    new ProjectNormals { ProjectId = 5, Month = 10, AvgHighTemp = 57.9m, AvgLowTemp = 38.8m, RecordAvgHighTemp = 64.8m, RecordAvgLowTemp = 36.2m, AvgRain = 2.7m, RecordHighAvgRain = 4.2m, RecordLowAvgRain = 0.6m, AvgSnow = 0.2m, RecordHighAvgSnow = 1m, RecordLowAvgSnow = 0m, AvgHighWind = 7.06m, AvgLowWind = 5.06m, RecordAvgHighWind = 9.06m, RecordAvgLowWind = 4.8m, },
                    new ProjectNormals { ProjectId = 5, Month = 11, AvgHighTemp = 45.6m, AvgLowTemp = 28m, RecordAvgHighTemp = 47.5m, RecordAvgLowTemp = 25.7m, AvgRain = 1.7m, RecordHighAvgRain = 3.2m, RecordLowAvgRain = 0.4m, AvgSnow = 1.7m, RecordHighAvgSnow = 3m, RecordLowAvgSnow = 0m, AvgHighWind = 8.04m, AvgLowWind = 6.04m, RecordAvgHighWind = 10.04m, RecordAvgLowWind = 5.4m, },
                    new ProjectNormals { ProjectId = 5, Month = 12, AvgHighTemp = 32.8m, AvgLowTemp = 15.7m, RecordAvgHighTemp = 36.5m, RecordAvgLowTemp = 6.1m, AvgRain = 2.7m, RecordHighAvgRain = 4.7m, RecordLowAvgRain = 1.3m, AvgSnow = 14.5m, RecordHighAvgSnow = 41.9m, RecordLowAvgSnow = 2m, AvgHighWind = 8.15m, AvgLowWind = 6.15m, RecordAvgHighWind = 10.15m, RecordAvgLowWind = 6.13m, },
                    new ProjectNormals { ProjectId = 6, Month = 1, AvgHighTemp = 63m, AvgLowTemp = 37m, RecordAvgHighTemp = 72.7m, RecordAvgLowTemp = 31.2m, AvgRain = 2.8m, RecordHighAvgRain = 7.7m, RecordLowAvgRain = 0.2m, AvgSnow = 0.1m, RecordHighAvgSnow = 1.3m, RecordLowAvgSnow = 0m, AvgHighWind = 10.58m, AvgLowWind = 8.58m, RecordAvgHighWind = 12.58m, RecordAvgLowWind = 8.35m, },
                    new ProjectNormals { ProjectId = 6, Month = 2, AvgHighTemp = 66.3m, AvgLowTemp = 41m, RecordAvgHighTemp = 76.5m, RecordAvgLowTemp = 32m, AvgRain = 1.8m, RecordHighAvgRain = 5.4m, RecordLowAvgRain = 0m, AvgSnow = 0.3m, RecordHighAvgSnow = 6.5m, RecordLowAvgSnow = 0m, AvgHighWind = 11.31m, AvgLowWind = 9.31m, RecordAvgHighWind = 13.31m, RecordAvgLowWind = 8.79m, },
                    new ProjectNormals { ProjectId = 6, Month = 3, AvgHighTemp = 73.9m, AvgLowTemp = 48.7m, RecordAvgHighTemp = 80.7m, RecordAvgLowTemp = 41.9m, AvgRain = 2.7m, RecordHighAvgRain = 6.2m, RecordLowAvgRain = 0.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 10.83m, AvgLowWind = 8.83m, RecordAvgHighWind = 12.83m, RecordAvgLowWind = 8.33m, },
                    new ProjectNormals { ProjectId = 6, Month = 4, AvgHighTemp = 80.6m, AvgLowTemp = 56.1m, RecordAvgHighTemp = 88m, RecordAvgLowTemp = 49.5m, AvgRain = 2.4m, RecordHighAvgRain = 6.5m, RecordLowAvgRain = 0.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 12.07m, AvgLowWind = 10.07m, RecordAvgHighWind = 14.07m, RecordAvgLowWind = 9.6m, },
                    new ProjectNormals { ProjectId = 6, Month = 5, AvgHighTemp = 87.5m, AvgLowTemp = 65m, RecordAvgHighTemp = 92.3m, RecordAvgLowTemp = 60.7m, AvgRain = 5m, RecordHighAvgRain = 15.8m, RecordLowAvgRain = 0.5m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.06m, AvgLowWind = 9.06m, RecordAvgHighWind = 13.06m, RecordAvgLowWind = 8.64m, },
                    new ProjectNormals { ProjectId = 6, Month = 6, AvgHighTemp = 93.7m, AvgLowTemp = 71.4m, RecordAvgHighTemp = 99.1m, RecordAvgLowTemp = 68.8m, AvgRain = 2.9m, RecordHighAvgRain = 14.2m, RecordLowAvgRain = 0.2m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 10.21m, AvgLowWind = 8.21m, RecordAvgHighWind = 12.21m, RecordAvgLowWind = 7.55m, },
                    new ProjectNormals { ProjectId = 6, Month = 7, AvgHighTemp = 96m, AvgLowTemp = 72.7m, RecordAvgHighTemp = 101.4m, RecordAvgLowTemp = 7m, AvgRain = 2.3m, RecordHighAvgRain = 7.6m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.32m, AvgLowWind = 7.32m, RecordAvgHighWind = 11.32m, RecordAvgLowWind = 6.78m, },
                    new ProjectNormals { ProjectId = 6, Month = 8, AvgHighTemp = 97.8m, AvgLowTemp = 72.7m, RecordAvgHighTemp = 105.2m, RecordAvgLowTemp = 69.8m, AvgRain = 2.2m, RecordHighAvgRain = 13m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 7.86m, AvgLowWind = 5.86m, RecordAvgHighWind = 9.86m, RecordAvgLowWind = 5.52m, },
                    new ProjectNormals { ProjectId = 6, Month = 9, AvgHighTemp = 92m, AvgLowTemp = 67.2m, RecordAvgHighTemp = 98.7m, RecordAvgLowTemp = 63.3m, AvgRain = 2.7m, RecordHighAvgRain = 7m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.73m, AvgLowWind = 6.73m, RecordAvgHighWind = 10.73m, RecordAvgLowWind = 5.59m, },
                    new ProjectNormals { ProjectId = 6, Month = 10, AvgHighTemp = 83.1m, AvgLowTemp = 56.9m, RecordAvgHighTemp = 91.2m, RecordAvgLowTemp = 51m, AvgRain = 4.3m, RecordHighAvgRain = 21.1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.03m, AvgLowWind = 6.03m, RecordAvgHighWind = 10.03m, RecordAvgLowWind = 5.81m, },
                    new ProjectNormals { ProjectId = 6, Month = 11, AvgHighTemp = 72.1m, AvgLowTemp = 46.8m, RecordAvgHighTemp = 79.5m, RecordAvgLowTemp = 41.6m, AvgRain = 2.7m, RecordHighAvgRain = 10.5m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.13m, AvgLowWind = 7.13m, RecordAvgHighWind = 11.13m, RecordAvgLowWind = 7.12m, },
                    new ProjectNormals { ProjectId = 6, Month = 12, AvgHighTemp = 64.8m, AvgLowTemp = 39.1m, RecordAvgHighTemp = 75m, RecordAvgLowTemp = 33.3m, AvgRain = 2.2m, RecordHighAvgRain = 6.4m, RecordLowAvgRain = 0.2m, AvgSnow = 0.1m, RecordHighAvgSnow = 1.3m, RecordLowAvgSnow = 0m, AvgHighWind = 8.73m, AvgLowWind = 6.73m, RecordAvgHighWind = 10.73m, RecordAvgLowWind = 6.27m, },
                    new ProjectNormals { ProjectId = 7, Month = 1, AvgHighTemp = 58.9m, AvgLowTemp = 35.5m, RecordAvgHighTemp = 69m, RecordAvgLowTemp = 28.1m, AvgRain = 2.8m, RecordHighAvgRain = 5.5m, RecordLowAvgRain = 0.3m, AvgSnow = 0.1m, RecordHighAvgSnow = 1m, RecordLowAvgSnow = 0m, AvgHighWind = 9.94m, AvgLowWind = 7.94m, RecordAvgHighWind = 11.94m, RecordAvgLowWind = 7.78m, },
                    new ProjectNormals { ProjectId = 7, Month = 2, AvgHighTemp = 62.1m, AvgLowTemp = 39.1m, RecordAvgHighTemp = 72.4m, RecordAvgLowTemp = 30.2m, AvgRain = 2.6m, RecordHighAvgRain = 8m, RecordLowAvgRain = 0.1m, AvgSnow = 0.3m, RecordHighAvgSnow = 4m, RecordLowAvgSnow = 0m, AvgHighWind = 11.63m, AvgLowWind = 9.63m, RecordAvgHighWind = 13.63m, RecordAvgLowWind = 9.11m, },
                    new ProjectNormals { ProjectId = 7, Month = 3, AvgHighTemp = 70.8m, AvgLowTemp = 46.9m, RecordAvgHighTemp = 79.2m, RecordAvgLowTemp = 38.8m, AvgRain = 3.5m, RecordHighAvgRain = 12m, RecordLowAvgRain = 0.1m, AvgSnow = 0.1m, RecordHighAvgSnow = 2m, RecordLowAvgSnow = 0m, AvgHighWind = 12.36m, AvgLowWind = 10.36m, RecordAvgHighWind = 14.36m, RecordAvgLowWind = 9.73m, },
                    new ProjectNormals { ProjectId = 7, Month = 4, AvgHighTemp = 77.4m, AvgLowTemp = 54.3m, RecordAvgHighTemp = 86.1m, RecordAvgLowTemp = 46.1m, AvgRain = 3.7m, RecordHighAvgRain = 9m, RecordLowAvgRain = 1.2m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 12.29m, AvgLowWind = 10.29m, RecordAvgHighWind = 14.29m, RecordAvgLowWind = 9.96m, },
                    new ProjectNormals { ProjectId = 7, Month = 5, AvgHighTemp = 84.6m, AvgLowTemp = 63.5m, RecordAvgHighTemp = 90.4m, RecordAvgLowTemp = 59.4m, AvgRain = 4.6m, RecordHighAvgRain = 14.5m, RecordLowAvgRain = 0.8m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.57m, AvgLowWind = 7.57m, RecordAvgHighWind = 11.57m, RecordAvgLowWind = 7.35m, },
                    new ProjectNormals { ProjectId = 7, Month = 6, AvgHighTemp = 91.9m, AvgLowTemp = 71.2m, RecordAvgHighTemp = 98.3m, RecordAvgLowTemp = 67.8m, AvgRain = 3.9m, RecordHighAvgRain = 13.5m, RecordLowAvgRain = 0.2m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.73m, AvgLowWind = 7.73m, RecordAvgHighWind = 11.73m, RecordAvgLowWind = 6.5m, },
                    new ProjectNormals { ProjectId = 7, Month = 7, AvgHighTemp = 96.2m, AvgLowTemp = 74.1m, RecordAvgHighTemp = 102.4m, RecordAvgLowTemp = 71.4m, AvgRain = 1.9m, RecordHighAvgRain = 5.4m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.93m, AvgLowWind = 6.93m, RecordAvgHighWind = 10.93m, RecordAvgLowWind = 6m, },
                    new ProjectNormals { ProjectId = 7, Month = 8, AvgHighTemp = 97.5m, AvgLowTemp = 74.3m, RecordAvgHighTemp = 106m, RecordAvgLowTemp = 70.1m, AvgRain = 1.9m, RecordHighAvgRain = 6.2m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 8.0m, AvgLowWind = 6m, RecordAvgHighWind = 10.0m, RecordAvgLowWind = 5.5m, },
                    new ProjectNormals { ProjectId = 7, Month = 9, AvgHighTemp = 90.9m, AvgLowTemp = 67.2m, RecordAvgHighTemp = 97.6m, RecordAvgLowTemp = 62.8m, AvgRain = 3.1m, RecordHighAvgRain = 8.1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 7.83m, AvgLowWind = 5.83m, RecordAvgHighWind = 9.83m, RecordAvgLowWind = 5.34m, },
                    new ProjectNormals { ProjectId = 7, Month = 10, AvgHighTemp = 81.1m, AvgLowTemp = 56.3m, RecordAvgHighTemp = 89.4m, RecordAvgLowTemp = 52.6m, AvgRain = 4.6m, RecordHighAvgRain = 13.3m, RecordLowAvgRain = 0.3m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.42m, AvgLowWind = 7.42m, RecordAvgHighWind = 11.42m, RecordAvgLowWind = 7.13m, },
                    new ProjectNormals { ProjectId = 7, Month = 11, AvgHighTemp = 69.8m, AvgLowTemp = 45.6m, RecordAvgHighTemp = 76.3m, RecordAvgLowTemp = 38.9m, AvgRain = 2.5m, RecordHighAvgRain = 10.1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.79m, AvgLowWind = 7.79m, RecordAvgHighWind = 11.79m, RecordAvgLowWind = 7.64m, },
                    new ProjectNormals { ProjectId = 7, Month = 12, AvgHighTemp = 61.5m, AvgLowTemp = 38m, RecordAvgHighTemp = 72.9m, RecordAvgLowTemp = 31.6m, AvgRain = 2.8m, RecordHighAvgRain = 9.7m, RecordLowAvgRain = 0.5m, AvgSnow = 0.2m, RecordHighAvgSnow = 3.5m, RecordLowAvgSnow = 0m, AvgHighWind = 9.28m, AvgLowWind = 7.28m, RecordAvgHighWind = 11.28m, RecordAvgLowWind = 7.21m, },
                    new ProjectNormals { ProjectId = 8, Month = 1, AvgHighTemp = 40.8m, AvgLowTemp = 13.6m, RecordAvgHighTemp = 47.8m, RecordAvgLowTemp = 13.6m, AvgRain = 2.3m, RecordHighAvgRain = 4.7m, RecordLowAvgRain = 1.1m, AvgSnow = 3.4m, RecordHighAvgSnow = 24m, RecordLowAvgSnow = 0m, AvgHighWind = 7.3m, AvgLowWind = 5.3m, RecordAvgHighWind = 9.3m, RecordAvgLowWind = 4.48m, },
                    new ProjectNormals { ProjectId = 8, Month = 2, AvgHighTemp = 44.6m, AvgLowTemp = 20.5m, RecordAvgHighTemp = 51.9m, RecordAvgLowTemp = 20.5m, AvgRain = 1.7m, RecordHighAvgRain = 3.3m, RecordLowAvgRain = 0.2m, AvgSnow = 1.2m, RecordHighAvgSnow = 13.8m, RecordLowAvgSnow = 0m, AvgHighWind = 9.12m, AvgLowWind = 7.12m, RecordAvgHighWind = 11.12m, RecordAvgLowWind = 6.88m, },
                    new ProjectNormals { ProjectId = 8, Month = 3, AvgHighTemp = 51.7m, AvgLowTemp = 30.8m, RecordAvgHighTemp = 57.7m, RecordAvgLowTemp = 30.8m, AvgRain = 2.1m, RecordHighAvgRain = 5.5m, RecordLowAvgRain = 0.6m, AvgSnow = 0.9m, RecordHighAvgSnow = 9.5m, RecordLowAvgSnow = 0m, AvgHighWind = 10.66m, AvgLowWind = 8.66m, RecordAvgHighWind = 12.66m, RecordAvgLowWind = 7.85m, },
                    new ProjectNormals { ProjectId = 8, Month = 4, AvgHighTemp = 58.9m, AvgLowTemp = 32.8m, RecordAvgHighTemp = 65.6m, RecordAvgLowTemp = 32.8m, AvgRain = 1.4m, RecordHighAvgRain = 2.9m, RecordLowAvgRain = 0.5m, AvgSnow = 0.1m, RecordHighAvgSnow = 1.7m, RecordLowAvgSnow = 0m, AvgHighWind = 10.61m, AvgLowWind = 8.61m, RecordAvgHighWind = 12.61m, RecordAvgLowWind = 8.04m, },
                    new ProjectNormals { ProjectId = 8, Month = 5, AvgHighTemp = 66.2m, AvgLowTemp = 37.9m, RecordAvgHighTemp = 71m, RecordAvgLowTemp = 37.9m, AvgRain = 1.6m, RecordHighAvgRain = 4.3m, RecordLowAvgRain = 0.7m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.43m, AvgLowWind = 9.43m, RecordAvgHighWind = 13.43m, RecordAvgLowWind = 8.15m, },
                    new ProjectNormals { ProjectId = 8, Month = 6, AvgHighTemp = 74.2m, AvgLowTemp = 45m, RecordAvgHighTemp = 78.6m, RecordAvgLowTemp = 45m, AvgRain = 1.2m, RecordHighAvgRain = 2.5m, RecordLowAvgRain = 0.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 10.64m, AvgLowWind = 8.64m, RecordAvgHighWind = 12.64m, RecordAvgLowWind = 8.06m, },
                    new ProjectNormals { ProjectId = 8, Month = 7, AvgHighTemp = 86.2m, AvgLowTemp = 47.1m, RecordAvgHighTemp = 91.5m, RecordAvgLowTemp = 47.1m, AvgRain = 0.4m, RecordHighAvgRain = 1m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 10.05m, AvgLowWind = 8.05m, RecordAvgHighWind = 12.05m, RecordAvgLowWind = 7.99m, },
                    new ProjectNormals { ProjectId = 8, Month = 8, AvgHighTemp = 85.9m, AvgLowTemp = 48.2m, RecordAvgHighTemp = 90.7m, RecordAvgLowTemp = 48.2m, AvgRain = 0.4m, RecordHighAvgRain = 1.7m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.33m, AvgLowWind = 7.33m, RecordAvgHighWind = 11.33m, RecordAvgLowWind = 7.07m, },
                    new ProjectNormals { ProjectId = 8, Month = 9, AvgHighTemp = 76m, AvgLowTemp = 40.1m, RecordAvgHighTemp = 81m, RecordAvgLowTemp = 40.1m, AvgRain = 0.6m, RecordHighAvgRain = 2.8m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.34m, AvgLowWind = 7.34m, RecordAvgHighWind = 11.34m, RecordAvgLowWind = 6.58m, },
                    new ProjectNormals { ProjectId = 8, Month = 10, AvgHighTemp = 61.3m, AvgLowTemp = 29.7m, RecordAvgHighTemp = 67.4m, RecordAvgLowTemp = 29.7m, AvgRain = 1.3m, RecordHighAvgRain = 2.6m, RecordLowAvgRain = 0.2m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.23m, AvgLowWind = 7.23m, RecordAvgHighWind = 11.23m, RecordAvgLowWind = 6.79m, },
                    new ProjectNormals { ProjectId = 8, Month = 11, AvgHighTemp = 48.6m, AvgLowTemp = 22.6m, RecordAvgHighTemp = 56.8m, RecordAvgLowTemp = 22.6m, AvgRain = 2.1m, RecordHighAvgRain = 4.4m, RecordLowAvgRain = 0.9m, AvgSnow = 1m, RecordHighAvgSnow = 6.5m, RecordLowAvgSnow = 0m, AvgHighWind = 9.15m, AvgLowWind = 7.15m, RecordAvgHighWind = 11.15m, RecordAvgLowWind = 5.63m, },
                    new ProjectNormals { ProjectId = 8, Month = 12, AvgHighTemp = 39.4m, AvgLowTemp = 18.8m, RecordAvgHighTemp = 43.1m, RecordAvgLowTemp = 18.8m, AvgRain = 2.4m, RecordHighAvgRain = 4m, RecordLowAvgRain = 1m, AvgSnow = 3.6m, RecordHighAvgSnow = 26.2m, RecordLowAvgSnow = 0m, AvgHighWind = 8.02m, AvgLowWind = 6.02m, RecordAvgHighWind = 10.02m, RecordAvgLowWind = 5.4m, },
                    new ProjectNormals { ProjectId = 9, Month = 1, AvgHighTemp = 57.9m, AvgLowTemp = 31.1m, RecordAvgHighTemp = 67.3m, RecordAvgLowTemp = 25.8m, AvgRain = 1m, RecordHighAvgRain = 3m, RecordLowAvgRain = 0m, AvgSnow = 0.7m, RecordHighAvgSnow = 9.5m, RecordLowAvgSnow = 0m, AvgHighWind = 11.74m, AvgLowWind = 9.74m, RecordAvgHighWind = 13.74m, RecordAvgLowWind = 9.61m, },
                    new ProjectNormals { ProjectId = 9, Month = 2, AvgHighTemp = 61.3m, AvgLowTemp = 33.7m, RecordAvgHighTemp = 70.8m, RecordAvgLowTemp = 22.3m, AvgRain = 1.5m, RecordHighAvgRain = 3.8m, RecordLowAvgRain = 0m, AvgSnow = 1.6m, RecordHighAvgSnow = 13.1m, RecordLowAvgSnow = 0m, AvgHighWind = 12.74m, AvgLowWind = 10.74m, RecordAvgHighWind = 14.74m, RecordAvgLowWind = 10.21m, },
                    new ProjectNormals { ProjectId = 9, Month = 3, AvgHighTemp = 70.4m, AvgLowTemp = 42.9m, RecordAvgHighTemp = 76.5m, RecordAvgLowTemp = 34.6m, AvgRain = 1.9m, RecordHighAvgRain = 5.4m, RecordLowAvgRain = 0.3m, AvgSnow = 0.1m, RecordHighAvgSnow = 1.3m, RecordLowAvgSnow = 0m, AvgHighWind = 13.74m, AvgLowWind = 11.74m, RecordAvgHighWind = 15.74m, RecordAvgLowWind = 10.26m, },
                    new ProjectNormals { ProjectId = 9, Month = 4, AvgHighTemp = 78.6m, AvgLowTemp = 50.7m, RecordAvgHighTemp = 88.2m, RecordAvgLowTemp = 43.9m, AvgRain = 2.4m, RecordHighAvgRain = 7.2m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 13.5m, AvgLowWind = 11.5m, RecordAvgHighWind = 15.5m, RecordAvgLowWind = 10.19m, },
                    new ProjectNormals { ProjectId = 9, Month = 5, AvgHighTemp = 84.9m, AvgLowTemp = 60.2m, RecordAvgHighTemp = 94.1m, RecordAvgLowTemp = 55.6m, AvgRain = 3.9m, RecordHighAvgRain = 11m, RecordLowAvgRain = 0.6m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 12.29m, AvgLowWind = 10.29m, RecordAvgHighWind = 14.29m, RecordAvgLowWind = 9.96m, },
                    new ProjectNormals { ProjectId = 9, Month = 6, AvgHighTemp = 92.1m, AvgLowTemp = 68.5m, RecordAvgHighTemp = 102.5m, RecordAvgLowTemp = 65.2m, AvgRain = 3.2m, RecordHighAvgRain = 7.2m, RecordLowAvgRain = 0.9m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 13.9m, AvgLowWind = 11.9m, RecordAvgHighWind = 15.9m, RecordAvgLowWind = 11.52m, },
                    new ProjectNormals { ProjectId = 9, Month = 7, AvgHighTemp = 95.1m, AvgLowTemp = 71.5m, RecordAvgHighTemp = 104.1m, RecordAvgLowTemp = 68.7m, AvgRain = 2.4m, RecordHighAvgRain = 11.6m, RecordLowAvgRain = 0m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.74m, AvgLowWind = 9.74m, RecordAvgHighWind = 13.74m, RecordAvgLowWind = 9.52m, },
                    new ProjectNormals { ProjectId = 9, Month = 8, AvgHighTemp = 95.4m, AvgLowTemp = 71m, RecordAvgHighTemp = 102.8m, RecordAvgLowTemp = 66.6m, AvgRain = 2.8m, RecordHighAvgRain = 10.1m, RecordLowAvgRain = 0.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 11.03m, AvgLowWind = 9.03m, RecordAvgHighWind = 13.03m, RecordAvgLowWind = 8.96m, },
                    new ProjectNormals { ProjectId = 9, Month = 9, AvgHighTemp = 88.5m, AvgLowTemp = 63.7m, RecordAvgHighTemp = 95m, RecordAvgLowTemp = 59m, AvgRain = 3.2m, RecordHighAvgRain = 10.6m, RecordLowAvgRain = 0.1m, AvgSnow = 0m, RecordHighAvgSnow = 0m, RecordLowAvgSnow = 0m, AvgHighWind = 9.9m, AvgLowWind = 7.9m, RecordAvgHighWind = 11.9m, RecordAvgLowWind = 7.57m, },
                    new ProjectNormals { ProjectId = 9, Month = 10, AvgHighTemp = 78.3m, AvgLowTemp = 52.7m, RecordAvgHighTemp = 85.9m, RecordAvgLowTemp = 47.4m, AvgRain = 3.1m, RecordHighAvgRain = 11.1m, RecordLowAvgRain = 0.3m, AvgSnow = 0.1m, RecordHighAvgSnow = 1.4m, RecordLowAvgSnow = 0m, AvgHighWind = 11.39m, AvgLowWind = 9.39m, RecordAvgHighWind = 13.39m, RecordAvgLowWind = 9.2m, },
                    new ProjectNormals { ProjectId = 9, Month = 11, AvgHighTemp = 67.6m, AvgLowTemp = 42m, RecordAvgHighTemp = 73m, RecordAvgLowTemp = 35.5m, AvgRain = 1.6m, RecordHighAvgRain = 5.5m, RecordLowAvgRain = 0m, AvgSnow = 0.6m, RecordHighAvgSnow = 6.6m, RecordLowAvgSnow = 0m, AvgHighWind = 11.6m, AvgLowWind = 9.6m, RecordAvgHighWind = 13.6m, RecordAvgLowWind = 8.94m, },
                    new ProjectNormals { ProjectId = 9, Month = 12, AvgHighTemp = 60.2m, AvgLowTemp = 33.8m, RecordAvgHighTemp = 74.4m, RecordAvgLowTemp = 29.2m, AvgRain = 1.1m, RecordHighAvgRain = 3.8m, RecordLowAvgRain = 0m, AvgSnow = 0.6m, RecordHighAvgSnow = 5.9m, RecordLowAvgSnow = 0m, AvgHighWind = 10.93m, AvgLowWind = 8.93m, RecordAvgHighWind = 12.93m, RecordAvgLowWind = 8.4m, }
                };

                var projectWindData = new List<ProjectWindData>()
                {
                    new ProjectWindData
                    {
                        ProjectId = 1,
                        ValidTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        Latitude = 40.994430m,
                        Longitude = -102.910430m,
                        U10 = 5.6,
                        V10 = -2.3,
                        Number = 1,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 1,
                        ValidTime = new DateTime(2024, 1, 1, 6, 0, 0, DateTimeKind.Utc),
                        Latitude = 40.994430m,
                        Longitude = -102.910430m,
                        U10 = 7.2,
                        V10 = -1.1,
                        Number = 2,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 2,
                        ValidTime = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                        Latitude = 37.756930m,
                        Longitude = -100.018880m,
                        U10 = 6.4,
                        V10 = -0.6,
                        Number = 3,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 2,
                        ValidTime = new DateTime(2024, 1, 2, 6, 0, 0, DateTimeKind.Utc),
                        Latitude = 37.756930m,
                        Longitude = -100.018880m,
                        U10 = 8.1,
                        V10 = 0.3,
                        Number = 4,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 3,
                        ValidTime = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                        Latitude = 41.657360m,
                        Longitude = -106.097330m,
                        U10 = 9.3,
                        V10 = -3.5,
                        Number = 5,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 3,
                        ValidTime = new DateTime(2024, 1, 3, 6, 0, 0, DateTimeKind.Utc),
                        Latitude = 41.657360m,
                        Longitude = -106.097330m,
                        U10 = 7.8,
                        V10 = -1.7,
                        Number = 6,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 4,
                        ValidTime = new DateTime(2024, 1, 4, 0, 0, 0, DateTimeKind.Utc),
                        Latitude = 33.622180m,
                        Longitude = -115.299230m,
                        U10 = 4.8,
                        V10 = 1.9,
                        Number = 7,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 4,
                        ValidTime = new DateTime(2024, 1, 4, 6, 0, 0, DateTimeKind.Utc),
                        Latitude = 33.622180m,
                        Longitude = -115.299230m,
                        U10 = 6.2,
                        V10 = 0.8,
                        Number = 8,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 5,
                        ValidTime = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                        Latitude = 43.003740m,
                        Longitude = -89.017500m,
                        U10 = 5.1,
                        V10 = -2.9,
                        Number = 9,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 5,
                        ValidTime = new DateTime(2024, 1, 5, 6, 0, 0, DateTimeKind.Utc),
                        Latitude = 43.003740m,
                        Longitude = -89.017500m,
                        U10 = 6.7,
                        V10 = -1.6,
                        Number = 10,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 6,
                        ValidTime = new DateTime(2024, 1, 6, 0, 0, 0, DateTimeKind.Utc),
                        Latitude = 30.267590m,
                        Longitude = -97.742990m,
                        U10 = 3.6,
                        V10 = 2.4,
                        Number = 11,
                        Expver = "0001"
                    },
                    new ProjectWindData
                    {
                        ProjectId = 6,
                        ValidTime = new DateTime(2024, 1, 6, 6, 0, 0, DateTimeKind.Utc),
                        Latitude = 30.267590m,
                        Longitude = -97.742990m,
                        U10 = 4.2,
                        V10 = 1.2,
                        Number = 12,
                        Expver = "0001"
                    }
                };

                context.ProjectNormals.AddRange(projectNormals);
                context.ProjectWindData.AddRange(projectWindData);
                _ = context.SaveChanges();
            }
        }
    }
}
