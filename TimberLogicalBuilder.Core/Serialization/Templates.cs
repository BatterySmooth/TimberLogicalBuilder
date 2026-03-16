namespace TimberLogicalBuilder.Core.Serialization;

public static class Templates
{
  public const string FactionName = "Folktails";
  public const string LeverTemplate = """
                                      {
                                        "Id": "{{ID}}",
                                        "Template": "Lever.{{FACTION}}",
                                        "Components": {
                                          "NamedEntity": {
                                            "EntityName": "{{NAME}}"
                                          },
                                          "BlockObject": {
                                            "Coordinates": {
                                              "X": "{{X}}",
                                              "Y": "{{Y}}",
                                              "Z": "{{Z}}"
                                            }
                                          },
                                          "Lever": {},
                                          "Automator": {
                                            "State": "Off"
                                          },
                                          "Inventory:ConstructionSite": {
                                            "Storage": {
                                              "Goods": [
                                                { "Good": "Plank", "Amount": 1 },
                                                { "Good": "Gear", "Amount": 2 }
                                              ]
                                            }
                                          }
                                        }
                                      }
                                      """;

  public const string RelayTemplate = """
                                      {
                                        "Id": "{{ID}}",
                                        "Template": "Relay.{{FACTION}}",
                                        "Components": {
                                          "NamedEntity": {
                                            "EntityName": "{{NAME}}"
                                          },
                                          "BlockObject": {
                                            "Coordinates": {
                                              "X": "{{X}}",
                                              "Y": "{{Y}}",
                                              "Z": "{{Z}}"
                                            }
                                          },
                                          "Relay": {
                                            "Mode": "{{MODE}}",
                                            "InputA": "{{INPUT_A}}"
                                          },
                                          "Automator": {
                                            "State": "Off"
                                          },
                                          "Inventory:ConstructionSite": {
                                            "Storage": {
                                              "Goods": [
                                                { "Good": "Plank", "Amount": 1 },
                                                { "Good": "Gear", "Amount": 1 }
                                              ]
                                            }
                                          }
                                        }
                                      }
                                      """;

  public const string MemoryTemplate = """
                                       {
                                         "Id": "{{ID}}",
                                         "Template": "Memory.{{FACTION}}",
                                         "Components": {
                                           "NamedEntity": {
                                             "EntityName": "{{NAME}}"
                                           },
                                           "BlockObject": {
                                             "Coordinates": {
                                               "X": "{{X}}",
                                               "Y": "{{Y}}",
                                               "Z": "{{Z}}"
                                             }
                                           },
                                           "Memory": {
                                             "Mode": "{{MODE}}",
                                             "InputA": "{{INPUT_A}}"
                                           },
                                           "Automator": {
                                             "State": "Off"
                                           },
                                           "Inventory:ConstructionSite": {
                                             "Storage": {
                                               "Goods": [
                                                 { "Good": "MetalBlock", "Amount": 1 },
                                                 { "Good": "Extract", "Amount": 1 }
                                               ]
                                             }
                                           }
                                         }
                                       }
                                       """;
  
  public const string TimerTemplate = """
                                      {
                                        "Id": "{{ID}}",
                                        "Template": "Timer.{{FACTION}}",
                                        "Components": {
                                          "NamedEntity": {
                                            "EntityName": "{{NAME}}"
                                          },
                                          "BlockObject": {
                                            "Coordinates": {
                                              "X": "{{X}}",
                                              "Y": "{{Y}}",
                                              "Z": "{{Z}}"
                                            }
                                          },
                                          "Timer": {
                                            "Mode": "{{MODE}}",
                                            "TimerIntervalA": {},
                                            "Input": "{{INPUT}}",
                                            "Counter": 0
                                          },
                                          "Automator": {
                                            "State": "Off"
                                          },
                                          "Inventory:ConstructionSite": {
                                            "Storage": {
                                              "Goods": [
                                                { "Good": "TreatedPlank", "Amount": 1 },
                                                { "Good": "MetalBlock", "Amount": 1 }
                                              ]
                                            }
                                          }
                                        }
                                      }
                                      """;

  public const string IndicatorTemplate = """
                                          {
                                            "Id": "{{ID}}",
                                            "Template": "Indicator.{{FACTION}}",
                                            "Components": {
                                              "NamedEntity": {
                                                "EntityName": "{{NAME}}"
                                              },
                                              "BlockObject": {
                                                "Coordinates": {
                                                  "X": "{{X}}",
                                                  "Y": "{{Y}}",
                                                  "Z": "{{Z}}"
                                                }
                                              },
                                              "Indicator": {},
                                              "CustomizableIlluminator": {
                                                "IsCustomized": true,
                                                "CustomColor": {
                                                  "r": 0.8509804,
                                                  "g": 0.490196079,
                                                  "b": 0.1254902,
                                                  "a": 1.0
                                                }
                                              },
                                              "Automatable": {
                                                "Input": "{{INPUT}}"
                                              },
                                              "Inventory:ConstructionSite": {
                                                "Storage": {
                                                  "Goods": [
                                                    { "Good": "ScrapMetal", "Amount": 2 },
                                                    { "Good": "PineResin", "Amount": 1 }
                                                  ]
                                                }
                                              }
                                            }
                                          }
                                          """;

  public const string HttpLeverTemplate = """
                                          {
                                            "Id": "{{ID}}",
                                            "Template": "HttpLever.{{FACTION}}",
                                            "Components": {
                                              "NamedEntity": {
                                                "EntityName": "{{NAME}}"
                                              },
                                              "BlockObject": {
                                                "Coordinates": {
                                                  "X":"{{X}}",
                                                  "Y": "{{Y}}",
                                                  "Z": "{{Z}}"
                                                },
                                                "Orientation": "Cw90"
                                              },
                                              "Lever": {
                                              },
                                              "Automator": {
                                                "State": "Off"
                                              },
                                              "Inventory:ConstructionSite": {
                                                "Storage": {
                                                  "Goods": [
                                                    { "Good": "Gear", "Amount": 2 },
                                                    { "Good": "ScrapMetal", "Amount": 2 }
                                                  ]
                                                }
                                              },
                                              "CustomizableIlluminator": {
                                                "IsCustomized": true,
                                                "CustomColor": {
                                                  "r": 1.0,
                                                  "g": 0.0,
                                                  "b": 0.0,
                                                  "a": 1.0
                                                }
                                              }
                                            }
                                          }
                                        """;

  public const string HttpAdapterTemplate = """
                                            {
                                              "Id": "{{ID}}",
                                              "Template": "HttpAdapter.{{FACTION}}",
                                              "Components": {
                                                "NamedEntity": {
                                                  "EntityName": "HTTP_out_0"
                                                },
                                                "BlockObject": {
                                                  "Coordinates": {
                                                    "X":"{{X}}",
                                                    "Y": "{{Y}}",
                                                    "Z": "{{Z}}"
                                                  },
                                                  "Orientation": "Cw90"
                                                },
                                                "HttpAdapter": {
                                                  "SwitchedOnWebbookUrlKey": "{{WHEN_ON_HTTP}}",
                                                  "SwitchedOffWebbookUrlKey": "{{WHEN_OFF_HTTP}}",
                                                  "MethodKey": "{{HTTP_MODE}}"
                                                },
                                                "Automatable": {
                                                  "Input": "{{INPUT}}"
                                                },
                                                "Inventory:ConstructionSite": {
                                                  "Storage": {
                                                    "Goods": [
                                                      { "Good": "Gear", "Amount": 2 },
                                                      { "Good": "ScrapMetal", "Amount": 4 }
                                                    ]
                                                  }
                                                }
                                              }
                                            }
                                            """;

  public const string PlatformTemplate = """
                                         {
                                           "Id": "{{ID}}",
                                           "Template": "Platform.{{FACTION}}",
                                           "Components": {
                                             "BlockObject": {
                                               "Coordinates": {
                                                 "X": "{{X}}",
                                                 "Y": "{{Y}}",
                                                 "Z": "{{Z}}"
                                               }
                                             },
                                             "Inventory:ConstructionSite": {
                                               "Storage": {
                                                 "Goods": [
                                                   { "Good": "Plank", "Amount": 6 }
                                                 ]
                                               }
                                             }
                                           }
                                         }
                                         """;

  public const string DoublePlatformTemplate = """
                                               {
                                                 "Id": "{{ID}}",
                                                 "Template": "DoublePlatform.{{FACTION}}",
                                                 "Components": {
                                                   "BlockObject": {
                                                     "Coordinates": {
                                                       "X": "{{X}}",
                                                       "Y": "{{Y}}",
                                                       "Z": "{{Z}}"
                                                     }
                                                   },
                                                   "Inventory:ConstructionSite": {
                                                     "Storage": {
                                                       "Goods": [
                                                         { "Good": "Plank", "Amount": 8 }
                                                       ]
                                                     }
                                                   }
                                                 }
                                               }
                                               """;
}