using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using UnityEngine;
using Verse;

namespace BiotechGeneSynthesis
{
	public class Dialog_CreateXenotype : GeneCreationDialogBase
	{
		// Token: 0x1700188F RID: 6287
		// (get) Token: 0x06008A5C RID: 35420 RVA: 0x002F7EC9 File Offset: 0x002F60C9
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2((float)Mathf.Min(UI.screenWidth, 1036), (float)(UI.screenHeight - 4));
			}
		}

		// Token: 0x17001890 RID: 6288
		// (get) Token: 0x06008A5D RID: 35421 RVA: 0x002F7EE8 File Offset: 0x002F60E8
		protected override List<GeneDef> SelectedGenes
		{
			get
			{
				return this.selectedGenes;
			}
		}

		// Token: 0x17001891 RID: 6289
		// (get) Token: 0x06008A5E RID: 35422 RVA: 0x002F7EF0 File Offset: 0x002F60F0
		protected override string Header
		{
			get
			{
				return "CreateXenotype".Translate().CapitalizeFirst();
			}
		}

		// Token: 0x17001892 RID: 6290
		// (get) Token: 0x06008A5F RID: 35423 RVA: 0x002F7F14 File Offset: 0x002F6114
		protected override string AcceptButtonLabel
		{
			get
			{
				return "SaveAndApply".Translate().CapitalizeFirst();
			}
		}

		// Token: 0x06008A60 RID: 35424 RVA: 0x002F7F38 File Offset: 0x002F6138
		public Dialog_CreateXenotype(int index, Action callback)
		{
			this.generationRequestIndex = index;
			this.callback = callback;
			this.xenotypeName = string.Empty;
			this.closeOnAccept = false;
			this.absorbInputAroundWindow = true;
			this.alwaysUseFullBiostatsTableHeight = true;
			this.searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
			foreach (GeneCategoryDef key in DefDatabase<GeneCategoryDef>.AllDefs)
			{
				this.collapsedCategories.Add(key, false);
			}
			this.OnGenesChanged();
		}

		// Token: 0x06008A61 RID: 35425 RVA: 0x002F8010 File Offset: 0x002F6210
		public override void PostOpen()
		{
			if (!ModLister.CheckBiotech("xenotype creation"))
			{
				this.Close(false);
				return;
			}
			base.PostOpen();
		}

		// Token: 0x06008A62 RID: 35426 RVA: 0x002F802C File Offset: 0x002F622C
		protected override void DrawGenes(Rect rect)
		{
			this.hoveredAnyGene = false;
			GUI.BeginGroup(rect);
			float num = 0f;
			this.DrawSection(new Rect(0f, 0f, rect.width, this.selectedHeight), this.selectedGenes, "SelectedGenes".Translate(), ref num, ref this.selectedHeight, false, rect, ref this.selectedCollapsed);
			if (!this.selectedCollapsed.Value)
			{
				num += 10f;
			}
			float num2 = num;
			Widgets.Label(0f, ref num, rect.width, "Genes".Translate().CapitalizeFirst(), default(TipSignal));
			num += 10f;
			float height = num - num2 - 4f;
			if (Widgets.ButtonText(new Rect(rect.width - 150f - 16f, num2, 150f, height), "CollapseAllCategories".Translate(), true, true, true, null))
			{
				SoundDefOf.TabClose.PlayOneShotOnCamera(null);
				foreach (GeneCategoryDef key in DefDatabase<GeneCategoryDef>.AllDefs)
				{
					this.collapsedCategories[key] = true;
				}
			}
			if (Widgets.ButtonText(new Rect(rect.width - 300f - 4f - 16f, num2, 150f, height), "ExpandAllCategories".Translate(), true, true, true, null))
			{
				SoundDefOf.TabOpen.PlayOneShotOnCamera(null);
				foreach (GeneCategoryDef key2 in DefDatabase<GeneCategoryDef>.AllDefs)
				{
					this.collapsedCategories[key2] = false;
				}
			}
			float num3 = num;
			Rect rect2 = new Rect(0f, num, rect.width - 16f, this.scrollHeight);
			Widgets.BeginScrollView(new Rect(0f, num, rect.width, rect.height - num), ref this.scrollPosition, rect2, true);
			Rect containingRect = rect2;
			containingRect.y = num + this.scrollPosition.y;
			containingRect.height = rect.height;
			bool? flag = null;
			this.DrawSection(rect, GeneUtility.GenesInOrder, null, ref num, ref this.unselectedHeight, true, containingRect, ref flag);
			if (Event.current.type == EventType.Layout)
			{
				this.scrollHeight = num - num3;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
			if (!this.hoveredAnyGene)
			{
				this.hoveredGene = null;
			}
		}

		// Token: 0x06008A63 RID: 35427 RVA: 0x002F82E8 File Offset: 0x002F64E8
		private void DrawSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect, ref bool? collapsed)
		{
			float num = 4f;
			if (!label.NullOrEmpty())
			{
				Rect rect2 = new Rect(0f, curY, rect.width, Text.LineHeight);
				rect2.xMax -= (adding ? 16f : (Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f));
				if (collapsed != null)
				{
					Rect position = new Rect(rect2.x, rect2.y + (rect2.height - 18f) / 2f, 18f, 18f);
					GUI.DrawTexture(position, collapsed.Value ? TexButton.Reveal : TexButton.Collapse);
					if (Widgets.ButtonInvisible(rect2, true))
					{
						collapsed = !collapsed;
						if (collapsed.Value)
						{
							SoundDefOf.TabClose.PlayOneShotOnCamera(null);
						}
						else
						{
							SoundDefOf.TabOpen.PlayOneShotOnCamera(null);
						}
					}
					if (Mouse.IsOver(rect2))
					{
						Widgets.DrawHighlight(rect2);
					}
					rect2.xMin += position.width;
				}
				Widgets.Label(rect2, label);
				if (!adding)
				{
					Text.Anchor = TextAnchor.UpperRight;
					GUI.color = ColoredText.SubtleGrayColor;
					Widgets.Label(new Rect(rect2.xMax - 18f, curY, rect.width - rect2.width, Text.LineHeight), "ClickToAddOrRemove".Translate());
					GUI.color = Color.white;
					Text.Anchor = TextAnchor.UpperLeft;
				}
				curY += Text.LineHeight + 3f;
			}
			bool? flag = collapsed;
			bool flag2 = true;
			if (flag.GetValueOrDefault() == flag2 & flag != null)
			{
				if (Event.current.type == EventType.Layout)
				{
					sectionHeight = 0f;
				}
				return;
			}
			float num2 = curY;
			bool flag3 = false;
			float num3 = 34f + GeneCreationDialogBase.GeneSize.x + 8f;
			float num4 = rect.width - 16f;
			float num5 = num3 + 4f;
			float b = (num4 - num5 * Mathf.Floor(num4 / num5)) / 2f;
			Rect rect3 = new Rect(0f, curY, rect.width, sectionHeight);
			if (!adding)
			{
				Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor, null);
			}
			curY += 4f;
			if (!genes.Any<GeneDef>())
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
				GUI.color = Color.white;
				Text.Anchor = TextAnchor.UpperLeft;
			}
			else
			{
				GeneCategoryDef geneCategoryDef = null;
				int num6 = 0;
				for (int i = 0; i < genes.Count; i++)
				{
					GeneDef geneDef = genes[i];
					if ((!adding || !this.quickSearchWidget.filter.Active || (this.matchingGenes.Contains(geneDef) && !this.selectedGenes.Contains(geneDef)) || this.matchingCategories.Contains(geneDef.displayCategory)) && (this.ignoreRestrictions || geneDef.biostatArc <= 0))
					{
						bool flag4 = false;
						if (num + num3 > num4)
						{
							num = 4f;
							curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
							flag4 = true;
						}
						bool flag5 = this.quickSearchWidget.filter.Active && (this.matchingGenes.Contains(geneDef) || this.matchingCategories.Contains(geneDef.displayCategory));
						bool flag6 = this.collapsedCategories[geneDef.displayCategory] && !flag5;
						if (adding && geneCategoryDef != geneDef.displayCategory)
						{
							if (!flag4 && flag3)
							{
								num = 4f;
								curY += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
							}
							geneCategoryDef = geneDef.displayCategory;
							Rect rect4 = new Rect(num, curY, rect.width - 8f, Text.LineHeight);
							if (!flag5)
							{
								Rect position2 = new Rect(rect4.x, rect4.y + (rect4.height - 18f) / 2f, 18f, 18f);
								GUI.DrawTexture(position2, flag6 ? TexButton.Reveal : TexButton.Collapse);
								if (Widgets.ButtonInvisible(rect4, true))
								{
									this.collapsedCategories[geneDef.displayCategory] = !this.collapsedCategories[geneDef.displayCategory];
									if (this.collapsedCategories[geneDef.displayCategory])
									{
										SoundDefOf.TabClose.PlayOneShotOnCamera(null);
									}
									else
									{
										SoundDefOf.TabOpen.PlayOneShotOnCamera(null);
									}
								}
								if (num6 % 2 == 1)
								{
									Widgets.DrawLightHighlight(rect4);
								}
								if (Mouse.IsOver(rect4))
								{
									Widgets.DrawHighlight(rect4);
								}
								rect4.xMin += position2.width;
							}
							Widgets.Label(rect4, geneCategoryDef.LabelCap);
							curY += rect4.height;
							if (!flag6)
							{
								GUI.color = Color.grey;
								Widgets.DrawLineHorizontal(num, curY, rect.width - 8f);
								GUI.color = Color.white;
								curY += 10f;
							}
							num6++;
						}
						if (adding && flag6)
						{
							flag3 = false;
							if (Event.current.type == EventType.Layout)
							{
								sectionHeight = curY - num2;
							}
						}
						else
						{
							num = Mathf.Max(num, b);
							flag3 = true;
							if (this.DrawGene(geneDef, !adding, ref num, curY, num3, containingRect, flag5))
							{
								if (this.selectedGenes.Contains(geneDef))
								{
									SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
									this.selectedGenes.Remove(geneDef);
								}
								else
								{
									SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
									this.selectedGenes.Add(geneDef);
								}
								if (!this.xenotypeNameLocked)
								{
									this.xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(this.SelectedGenes);
								}
								this.OnGenesChanged();
								break;
							}
						}
					}
				}
			}
			if (!adding || flag3)
			{
				curY += GeneCreationDialogBase.GeneSize.y + 12f;
			}
			if (Event.current.type == EventType.Layout)
			{
				sectionHeight = curY - num2;
			}
		}

		// Token: 0x06008A64 RID: 35428 RVA: 0x002F8954 File Offset: 0x002F6B54
		private bool DrawGene(GeneDef geneDef, bool selectedSection, ref float curX, float curY, float packWidth, Rect containingRect, bool isMatch)
		{
			bool result = false;
			Rect rect = new Rect(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
			if (!containingRect.Overlaps(rect))
			{
				curX = rect.xMax + 4f;
				return false;
			}
			bool selected = !selectedSection && this.selectedGenes.Contains(geneDef);
			bool overridden = this.leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef));
			Widgets.DrawOptionBackground(rect, selected);
			curX += 4f;
			GeneUIUtility.DrawBiostats(geneDef.biostatCpx, geneDef.biostatMet, geneDef.biostatArc, ref curX, curY, 4f);
			Rect rect2 = new Rect(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
			if (isMatch)
			{
				Widgets.DrawStrongHighlight(rect2.ExpandedBy(6f), null);
			}
			GeneUIUtility.DrawGeneDef(geneDef, rect2, this.inheritable ? GeneType.Endogene : GeneType.Xenogene, this.GeneTip(geneDef, selectedSection), false, false, overridden);
			curX += GeneCreationDialogBase.GeneSize.x + 4f;
			if (Mouse.IsOver(rect))
			{
				this.hoveredGene = geneDef;
				this.hoveredAnyGene = true;
			}
			else if (this.hoveredGene != null && geneDef.ConflictsWith(this.hoveredGene))
			{
				Widgets.DrawLightHighlight(rect);
			}
			if (Widgets.ButtonInvisible(rect, true))
			{
				result = true;
			}
			curX = Mathf.Max(curX, rect.xMax + 4f);
			return result;
		}

		// Token: 0x06008A65 RID: 35429 RVA: 0x002F8AFC File Offset: 0x002F6CFC
		private string GeneTip(GeneDef geneDef, bool selectedSection)
		{
			string text = null;
			if (selectedSection)
			{
				if (this.leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == geneDef))
				{
					text = Dialog_CreateXenotype.< GeneTip > g__GroupInfo | 25_0(this.leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == geneDef));
				}
				else if (this.cachedOverriddenGenes.Contains(geneDef))
				{
					text = Dialog_CreateXenotype.< GeneTip > g__GroupInfo | 25_0(this.leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef)));
				}
				else if (this.randomChosenGroups.ContainsKey(geneDef))
				{
					text = ("GeneWillBeRandomChosen".Translate() + ":\n" + (from x in this.randomChosenGroups[geneDef]
																			select x.label).ToLineList("  - ", true)).Colorize(ColoredText.TipSectionTitleColor);
				}
			}
			if (this.selectedGenes.Contains(geneDef) && geneDef.prerequisite != null && !this.selectedGenes.Contains(geneDef.prerequisite))
			{
				if (!text.NullOrEmpty())
				{
					text += "\n\n";
				}
				text += ("MessageGeneMissingPrerequisite".Translate(geneDef.label).CapitalizeFirst() + ": " + geneDef.prerequisite.LabelCap).Colorize(ColorLibrary.RedReadable);
			}
			if (!text.NullOrEmpty())
			{
				text += "\n\n";
			}
			return text + (this.selectedGenes.Contains(geneDef) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
		}

		// Token: 0x06008A66 RID: 35430 RVA: 0x002F8CF0 File Offset: 0x002F6EF0
		protected override void PostXenotypeOnGUI(float curX, float curY)
		{
			TaggedString taggedString = "GenesAreInheritable".Translate();
			TaggedString taggedString2 = "IgnoreRestrictions".Translate();
			float width = Mathf.Max(Text.CalcSize(taggedString).x, Text.CalcSize(taggedString2).x) + 4f + 24f;
			Rect rect = new Rect(curX, curY, width, Text.LineHeight);
			Widgets.CheckboxLabeled(rect, taggedString, ref this.inheritable, false, null, null, false);
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "GenesAreInheritableDesc".Translate());
			}
			rect.y += Text.LineHeight;
			bool ignoreRestrictions = this.ignoreRestrictions;
			Widgets.CheckboxLabeled(rect, taggedString2, ref this.ignoreRestrictions, false, null, null, false);
			if (ignoreRestrictions != this.ignoreRestrictions)
			{
				if (this.ignoreRestrictions)
				{
					if (!Dialog_CreateXenotype.ignoreRestrictionsConfirmationSent)
					{
						Dialog_CreateXenotype.ignoreRestrictionsConfirmationSent = true;
						Find.WindowStack.Add(new Dialog_MessageBox("IgnoreRestrictionsConfirmation".Translate(), "Yes".Translate(), delegate ()
						{
						}, "No".Translate(), delegate ()
						{
							this.ignoreRestrictions = false;
						}, null, false, null, null, WindowLayer.Dialog));
					}
				}
				else
				{
					this.selectedGenes.RemoveAll((GeneDef x) => x.biostatArc > 0);
					this.OnGenesChanged();
				}
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, "IgnoreRestrictionsDesc".Translate());
			}
			this.postXenotypeHeight += rect.yMax - curY;
		}

		// Token: 0x06008A67 RID: 35431 RVA: 0x002F8EB2 File Offset: 0x002F70B2
		protected override void OnGenesChanged()
		{
			this.selectedGenes.SortGeneDefs();
			base.OnGenesChanged();
		}

		// Token: 0x06008A68 RID: 35432 RVA: 0x002F8EC8 File Offset: 0x002F70C8
		protected override void DrawSearchRect(Rect rect)
		{
			base.DrawSearchRect(rect);
			if (Widgets.ButtonText(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x, rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadCustom".Translate(), true, true, true, null))
			{
				Find.WindowStack.Add(new Dialog_XenotypeList_Load(delegate (CustomXenotype xenotype)
				{
					this.xenotypeName = xenotype.name;
					this.xenotypeNameLocked = true;
					this.selectedGenes.Clear();
					this.selectedGenes.AddRange(xenotype.genes);
					this.inheritable = xenotype.inheritable;
					this.iconDef = xenotype.IconDef;
					this.OnGenesChanged();
					this.ignoreRestrictions = (this.selectedGenes.Any((GeneDef x) => x.biostatArc > 0) || !this.WithinAcceptableBiostatLimits(false));
				}));
			}
			if (Widgets.ButtonText(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x * 2f - 4f, rect.y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadPremade".Translate(), true, true, true, null))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (XenotypeDef xenotype2 in from c in DefDatabase<XenotypeDef>.AllDefs
												  orderby -c.displayPriority
												  select c)
				{
					XenotypeDef xenotype = xenotype2;
					list.Add(new FloatMenuOption(xenotype.LabelCap, delegate ()
					{
						this.xenotypeName = xenotype.label;
						this.selectedGenes.Clear();
						this.selectedGenes.AddRange(xenotype.genes);
						this.inheritable = xenotype.inheritable;
						this.OnGenesChanged();
						this.ignoreRestrictions = (this.selectedGenes.Any((GeneDef g) => g.biostatArc > 0) || !this.WithinAcceptableBiostatLimits(false));
					}, xenotype.Icon, XenotypeDef.IconColor, MenuOptionPriority.Default, delegate (Rect r)
					{
						TooltipHandler.TipRegion(r, xenotype.descriptionShort ?? xenotype.description);
					}, null, 0f, null, null, true, 0, HorizontalJustification.Left, false));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		// Token: 0x06008A69 RID: 35433 RVA: 0x002F908C File Offset: 0x002F728C
		protected override void DoBottomButtons(Rect rect)
		{
			base.DoBottomButtons(rect);
			if (this.leftChosenGroups.Any<GeneLeftChosenGroup>())
			{
				int num = this.leftChosenGroups.Sum((GeneLeftChosenGroup x) => x.overriddenGenes.Count);
				GeneLeftChosenGroup geneLeftChosenGroup = this.leftChosenGroups[0];
				string text = "GenesConflict".Translate() + ": " + "GenesConflictDesc".Translate(geneLeftChosenGroup.leftChosen.Named("FIRST"), geneLeftChosenGroup.overriddenGenes[0].Named("SECOND")).CapitalizeFirst() + ((num > 1) ? (" +" + (num - 1)) : string.Empty);
				float x2 = Text.CalcSize(text).x;
				GUI.color = ColorLibrary.RedReadable;
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(new Rect(rect.xMax - GeneCreationDialogBase.ButSize.x - x2 - 4f, rect.y, x2, rect.height), text);
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
			}
		}

		// Token: 0x06008A6A RID: 35434 RVA: 0x002F91C4 File Offset: 0x002F73C4
		protected override bool CanAccept()
		{
			if (!base.CanAccept())
			{
				return false;
			}
			if (!this.selectedGenes.Any<GeneDef>())
			{
				Messages.Message("MessageNoSelectedGenes".Translate(), null, MessageTypeDefOf.RejectInput, false);
				return false;
			}
			if (GenFilePaths.AllCustomXenotypeFiles.EnumerableCount() >= 200)
			{
				Messages.Message("MessageTooManyCustomXenotypes", null, MessageTypeDefOf.RejectInput, false);
				return false;
			}
			if (!this.ignoreRestrictions && this.leftChosenGroups.Any<GeneLeftChosenGroup>())
			{
				Messages.Message("MessageConflictingGenesPresent".Translate(), null, MessageTypeDefOf.RejectInput, false);
				return false;
			}
			return true;
		}

		// Token: 0x06008A6B RID: 35435 RVA: 0x002F925C File Offset: 0x002F745C
		protected override void Accept()
		{
			IEnumerable<string> warnings = this.GetWarnings();
			if (warnings.Any<string>())
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("XenotypeBreaksLimits".Translate() + ":\n" + warnings.ToLineList("  - ", true) + "\n\n" + "SaveAnyway".Translate(), new Action(this.AcceptInner), false, null, WindowLayer.Dialog));
				return;
			}
			this.AcceptInner();
		}

		// Token: 0x06008A6C RID: 35436 RVA: 0x002F92DC File Offset: 0x002F74DC
		private void AcceptInner()
		{
			CustomXenotype customXenotype = new CustomXenotype();
			CustomXenotype customXenotype2 = customXenotype;
			string xenotypeName = this.xenotypeName;
			customXenotype2.name = ((xenotypeName != null) ? xenotypeName.Trim() : null);
			customXenotype.genes.AddRange(this.selectedGenes);
			customXenotype.inheritable = this.inheritable;
			customXenotype.iconDef = this.iconDef;
			string xenotypeName2 = GenFile.SanitizedFileName(customXenotype.name);
			string absPath = GenFilePaths.AbsFilePathForXenotype(xenotypeName2);
			LongEventHandler.QueueLongEvent(delegate ()
			{
				GameDataSaveLoader.SaveXenotype(customXenotype, absPath);
			}, "SavingLongEvent", false, null, true);
			PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(this.generationRequestIndex);
			generationRequest.ForcedXenotype = null;
			generationRequest.ForcedCustomXenotype = customXenotype;
			StartingPawnUtility.SetGenerationRequest(this.generationRequestIndex, generationRequest);
			Action action = this.callback;
			if (action != null)
			{
				action();
			}
			this.Close(true);
		}

		// Token: 0x06008A6D RID: 35437 RVA: 0x002F93CA File Offset: 0x002F75CA
		private IEnumerable<string> GetWarnings()
		{
			if (!this.ignoreRestrictions)
			{
				yield break;
			}
			if (this.arc > 0 && this.inheritable)
			{
				yield return "XenotypeBreaksLimits_Archites".Translate();
			}
			if (this.met > GeneTuning.BiostatRange.TrueMax)
			{
				yield return "XenotypeBreaksLimits_Exceeds".Translate("Metabolism".Translate().ToLower().Named("STAT"), this.met.Named("VALUE"), GeneTuning.BiostatRange.TrueMax.Named("MAX"));
			}
			else if (this.met < GeneTuning.BiostatRange.TrueMin)
			{
				yield return "XenotypeBreaksLimits_Below".Translate("Metabolism".Translate().ToLower().Named("STAT"), this.met.Named("VALUE"), GeneTuning.BiostatRange.TrueMin.Named("MIN"));
			}
			yield break;
		}

		// Token: 0x06008A6E RID: 35438 RVA: 0x002F93DC File Offset: 0x002F75DC
		protected override void UpdateSearchResults()
		{
			this.quickSearchWidget.noResultsMatched = false;
			this.matchingGenes.Clear();
			this.matchingCategories.Clear();
			if (!this.quickSearchWidget.filter.Active)
			{
				return;
			}
			foreach (GeneDef geneDef in GeneUtility.GenesInOrder)
			{
				if (!this.selectedGenes.Contains(geneDef))
				{
					if (this.quickSearchWidget.filter.Matches(geneDef.label))
					{
						this.matchingGenes.Add(geneDef);
					}
					if (this.quickSearchWidget.filter.Matches(geneDef.displayCategory.label) && !this.matchingCategories.Contains(geneDef.displayCategory))
					{
						this.matchingCategories.Add(geneDef.displayCategory);
					}
				}
			}
			this.quickSearchWidget.noResultsMatched = (!this.matchingGenes.Any<GeneDef>() && !this.matchingCategories.Any<GeneCategoryDef>());
		}

		// Token: 0x06008A70 RID: 35440 RVA: 0x002F951C File Offset: 0x002F771C
		

	// Token: 0x04004C9E RID: 19614
	private int generationRequestIndex;

	// Token: 0x04004C9F RID: 19615
	private Action callback;

	// Token: 0x04004CA0 RID: 19616
	private List<GeneDef> selectedGenes = new List<GeneDef>();

	// Token: 0x04004CA1 RID: 19617
	private bool inheritable;

	// Token: 0x04004CA2 RID: 19618
	private bool? selectedCollapsed = new bool?(false);

	// Token: 0x04004CA3 RID: 19619
	private List<GeneCategoryDef> matchingCategories = new List<GeneCategoryDef>();

	// Token: 0x04004CA4 RID: 19620
	private Dictionary<GeneCategoryDef, bool> collapsedCategories = new Dictionary<GeneCategoryDef, bool>();

	// Token: 0x04004CA5 RID: 19621
	private bool hoveredAnyGene;

	// Token: 0x04004CA6 RID: 19622
	private GeneDef hoveredGene;

	// Token: 0x04004CA7 RID: 19623
	private static bool ignoreRestrictionsConfirmationSent;

	// Token: 0x04004CA8 RID: 19624
	private const int MaxCustomXenotypes = 200;

	// Token: 0x04004CA9 RID: 19625
	private static readonly Color OutlineColorSelected = new Color(1f, 1f, 0.7f, 1f);
	}
}
