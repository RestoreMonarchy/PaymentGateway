﻿CREATE TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS] (
    [SCHED_NAME]    NVARCHAR (120) NOT NULL,
    [TRIGGER_GROUP] NVARCHAR (150) NOT NULL,
    CONSTRAINT [PK_QRTZ_PAUSED_TRIGGER_GRPS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [TRIGGER_GROUP] ASC)
);

