SET IDENTITY_INSERT [dbo].[Billet] ON
INSERT INTO [dbo].[Billet] ([Id], [AuteurId], [Commentaires], [CompteId], [DepartementId], [Description], [EquipeId], [Etat], [Image], [Titre]) VALUES (1, 2, NULL, NULL, 1, N'HEYHO', NULL, N'Nouveau', N'./images/billet2-0', N'Test')
SET IDENTITY_INSERT [dbo].[Billet] OFF
