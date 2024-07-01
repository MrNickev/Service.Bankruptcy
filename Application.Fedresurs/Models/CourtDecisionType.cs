namespace Application.Fedresurs.Models;

/// <summary>
/// Типы актов
/// </summary>
public enum CourtDecisionType
{
    /// <summary>
    /// о введении внешнего управления 
    /// </summary>
    ExternalManagement,
    
    /// <summary>
    /// об отмене или изменении судебных актов
    /// </summary>
    ChangeCourtAct,
    
    /// <summary>
    /// о возобновлении производства по делу о несостоятельности (банкротстве)
    /// </summary>
    LegalCaseResume,
    
    /// <summary>
    /// об утверждении арбитражного управляющего
    /// </summary>
    ArbitrManagerApproval,
    
    /// <summary>
    /// об освобождении или отстранении арбитражного управляющего
    /// </summary>
    ArbitrManagerRelease,
    
    /// <summary>
    /// о признании должника банкротом и открытии конкурсного производства
    /// </summary>
    Receivership,
    
    /// <summary>
    /// о прекращении производства по делу
    /// </summary>
    LegalCaseTermination,
    
    /// <summary>
    /// о введении финансового оздоровления
    /// </summary>
    FinancialRecovery,
    
    /// <summary>
    /// об отказе в признании должника банкротом
    /// </summary>
    BankruptcyRefusal,
    
    /// <summary>
    /// о введении наблюдения
    /// </summary>
    Observation,
    
    /// <summary>
    /// Другие судебные акты 
    /// </summary>
    OtherAct,
    
    /// <summary>
    /// Другие определения
    /// </summary>
    OtherDefinition,
    
    /// <summary>
    /// об удовлетворении заявлений третьих лиц о намерении погасить обязательства должника
    /// </summary>
    ThirdPartyPayoffRequestApproval,
    
    /// <summary>
    /// о признании обоснованным заявления о признании гражданина банкротом и
    /// введении реструктуризации его долгов
    /// </summary>
    DebtRestructuring,
    
    /// <summary>
    /// о признании гражданина банкротом и введении реализации имущества гражданина
    /// </summary>
    PropertySale,
    
    /// <summary>
    /// об утверждении плана реструктуризации долгов гражданина
    /// </summary>
    DebtRestructuringPlan,
    
    /// <summary>
    /// о завершении реструктуризации долгов гражданина
    /// </summary>
    DebtRestructuringComplete,
    
    /// <summary>
    /// о признании действий (бездействий) арбитражного управляющего незаконными
    /// </summary>
    ArbitrManagerActionsIllegal,
    
    /// <summary>
    /// о взыскании с арбитражного управляющего убытков в связи с неисполнением или
    /// ненадлежащим исполнением обязанностей
    /// </summary>
    ArbitrManagerLossesRecovery,
    
    /// <summary>
    /// о неприменении в отношении гражданина правила об освобождении от
    /// исполнения обязательств
    /// </summary>
    ObligationsDischargeRefusal,
    
    /// <summary>
    /// о завершении реализации имущества гражданина
    /// </summary>
    PropertySaleComplete,
    
    /// <summary>
    /// о применении при банкротстве должника правил параграфа «Банкротство застройщиков»
    /// </summary>
    DeveloperBankruptcy,
    
    /// <summary>
    /// о передаче дела на рассмотрение другого арбитражного суда
    /// </summary>
    CaseTransfer,
    
    /// <summary>
    /// о завершении конкурсного производства 
    /// </summary>
    LegalCaseEnd,
    
    /// <summary>
    /// о продлении срока процедуры 
    /// </summary>
    ExtensionProcedure,
    
    /// <summary>
    /// об изменении судебного акта
    /// </summary>
    ChangeArbitralDecree,
    
    /// <summary>
    /// об отмене судебного акта 
    /// </summary>
    CancelArbitralDecree
}