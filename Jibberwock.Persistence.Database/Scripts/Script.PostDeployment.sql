﻿/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\Lookups\components.Purpose.sql
:r .\Lookups\components.StatusProvider.sql
:r .\Lookups\core.EmailBatchType.sql
:r .\Lookups\core.NotificationPriority.sql
:r .\Lookups\core.NotificationStatus.sql
:r .\Lookups\core.NotificationType.sql
:r .\Lookups\products.CharacteristicValueType.sql
:r .\Lookups\security.AuditTrailType.sql
:r .\Lookups\security.ExternalIdentityProvider.sql
:r .\Lookups\security.SecurableResourceType.sql
:r .\Lookups\security.Permission.sql
:r .\Lookups\security.UserType.sql
:r .\Lookups\security.WellKnownGroupType.sql
:r .\Lookups\tenants.InvitationStatus.sql
:r .\Lookups\tenants.SubscriptionStatus.sql

:r .\StaticData\components.ExternalComponent.sql
:r .\StaticData\core.Service.sql
:r .\StaticData\security.SecurityGroup.sql
