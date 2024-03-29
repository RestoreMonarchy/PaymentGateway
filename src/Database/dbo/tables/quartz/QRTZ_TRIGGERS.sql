﻿CREATE TABLE [dbo].[QRTZ_TRIGGERS] (
    [SCHED_NAME]     NVARCHAR (120)  NOT NULL,
    [TRIGGER_NAME]   NVARCHAR (150)  NOT NULL,
    [TRIGGER_GROUP]  NVARCHAR (150)  NOT NULL,
    [JOB_NAME]       NVARCHAR (150)  NOT NULL,
    [JOB_GROUP]      NVARCHAR (150)  NOT NULL,
    [DESCRIPTION]    NVARCHAR (250)  NULL,
    [NEXT_FIRE_TIME] BIGINT          NULL,
    [PREV_FIRE_TIME] BIGINT          NULL,
    [PRIORITY]       INT             NULL,
    [TRIGGER_STATE]  NVARCHAR (16)   NOT NULL,
    [TRIGGER_TYPE]   NVARCHAR (8)    NOT NULL,
    [START_TIME]     BIGINT          NOT NULL,
    [END_TIME]       BIGINT          NULL,
    [CALENDAR_NAME]  NVARCHAR (200)  NULL,
    [MISFIRE_INSTR]  INT             NULL,
    [JOB_DATA]       VARBINARY (MAX) NULL,
    CONSTRAINT [PK_QRTZ_TRIGGERS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [TRIGGER_NAME] ASC, [TRIGGER_GROUP] ASC),
    CONSTRAINT [FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS] FOREIGN KEY ([SCHED_NAME], [JOB_NAME], [JOB_GROUP]) REFERENCES [dbo].[QRTZ_JOB_DETAILS] ([SCHED_NAME], [JOB_NAME], [JOB_GROUP])
);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_G_J]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [JOB_GROUP] ASC, [JOB_NAME] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_C]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [CALENDAR_NAME] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_N_G_STATE]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [TRIGGER_GROUP] ASC, [TRIGGER_STATE] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_STATE]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [TRIGGER_STATE] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_N_STATE]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [TRIGGER_NAME] ASC, [TRIGGER_GROUP] ASC, [TRIGGER_STATE] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_NEXT_FIRE_TIME]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [NEXT_FIRE_TIME] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_NFT_ST]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [TRIGGER_STATE] ASC, [NEXT_FIRE_TIME] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_NFT_ST_MISFIRE]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [MISFIRE_INSTR] ASC, [NEXT_FIRE_TIME] ASC, [TRIGGER_STATE] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_QRTZ_T_NFT_ST_MISFIRE_GRP]
    ON [dbo].[QRTZ_TRIGGERS]([SCHED_NAME] ASC, [MISFIRE_INSTR] ASC, [NEXT_FIRE_TIME] ASC, [TRIGGER_GROUP] ASC, [TRIGGER_STATE] ASC);

