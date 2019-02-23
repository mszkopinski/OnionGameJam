using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CombatTest : SerializedMonoBehaviour {
	public class Bot {
		public Vector2Int pos;
		public int type;
	}

	[LabelText("Flat")]
	public GameObject tile;
	[LabelText("Spawn")]
	public GameObject player;
	[LabelText("Exit")]
	public GameObject exit;
	[LabelText("Wolf")]
	public GameObject enemy1;
	[LabelText("Golem")]
	public GameObject enemy2;
	[LabelText("Passive")]
	public GameObject enemy3;
	
	[LabelText("Object Holder")]
	public Transform objHold;

	public int[,] layerPattern = new int[10, 10];

	public int[,] currentState	= new int[10, 10];

	private Vector2Int playerPos = new Vector2Int();

	public List<Bot> bots = new List<Bot>();

	private void Start() {
		Generate();
	}

	private void Update() {
		if (Input.GetKeyDown("w")) {
			if (currentState[playerPos.x, (playerPos + Vector2Int.up).y] == 0) {
				Debug.Log("GAME OVER");
				playerPos += Vector2Int.up;
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.up).y] == 1) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;
				}
				
				currentState[playerPos.x, (playerPos + Vector2Int.up).y] = 2;
				playerPos += Vector2Int.up;
				DoSomeAI();
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.up).y] == 3) {
				Debug.Log("ALMOST THERE");
				currentState[playerPos.x, playerPos.y] = 1;
				currentState[playerPos.x, (playerPos + Vector2Int.up).y] = 7;
				playerPos += Vector2Int.up;
				DoSomeAI();
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.up).y] == 4) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[playerPos.x, (playerPos + Vector2Int.up + Vector2Int.up).y] == 0) {
					currentState[playerPos.x, (playerPos + Vector2Int.up).y] = 1;
					bots.RemoveAll(bot => bot.pos.x == playerPos.x && (playerPos + Vector2Int.up).y == bot.pos.y);
				}
				else if (currentState[playerPos.x, (playerPos + Vector2Int.up + Vector2Int.up).y] == 1) {
					currentState[playerPos.x, (playerPos + Vector2Int.up + Vector2Int.up).y] = 4;
					bots.Find((bot) => playerPos.x == bot.pos.x && (playerPos + Vector2Int.up).y == bot.pos.y).pos += Vector2Int.up;
					currentState[playerPos.x, (playerPos + Vector2Int.up).y] = 1;
				}
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.up).y] == 5) {
				DoSomeAI();
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.up).y] == 6) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[playerPos.x, (playerPos + Vector2Int.up + Vector2Int.up).y] == 0) {
					currentState[playerPos.x, (playerPos + Vector2Int.up).y] = 1;
					bots.RemoveAll(bot => bot.pos.x == playerPos.x && (playerPos + Vector2Int.up).y == bot.pos.y);
				}
				else if (currentState[playerPos.x, (playerPos + Vector2Int.up + Vector2Int.up).y] == 1) {
					currentState[playerPos.x, (playerPos + Vector2Int.up + Vector2Int.up).y] = 6;
					currentState[playerPos.x, (playerPos + Vector2Int.up).y] = 1;
				}
			}
			RefreshScene();
		}
		else if (Input.GetKeyDown("s")) {
			if (currentState[playerPos.x, (playerPos + Vector2Int.down).y] == 0) {
				Debug.Log("GAME OVER");
				playerPos += Vector2Int.down;
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.down).y] == 1) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;
				}
				currentState[playerPos.x, (playerPos + Vector2Int.down).y] = 2;
				playerPos += Vector2Int.down;
				DoSomeAI();
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.down).y] == 3) {
				Debug.Log("ALMOST THERE");
				currentState[playerPos.x, playerPos.y] = 1;
				currentState[playerPos.x, (playerPos + Vector2Int.down).y] = 7;
				playerPos += Vector2Int.down;
				DoSomeAI();
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.down).y] == 4) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[playerPos.x, (playerPos + Vector2Int.down + Vector2Int.down).y] == 0) {
					currentState[playerPos.x, (playerPos + Vector2Int.down).y] = 1;
					bots.RemoveAll(bot => bot.pos.x == playerPos.x && (playerPos + Vector2Int.down).y == bot.pos.y);
				}
				else if (currentState[playerPos.x, (playerPos + Vector2Int.down + Vector2Int.down).y] == 1) {
					currentState[playerPos.x, (playerPos + Vector2Int.down + Vector2Int.down).y] = 4;
					bots.Find((bot) => playerPos.x == bot.pos.x && (playerPos + Vector2Int.down).y == bot.pos.y).pos += Vector2Int.down;
					currentState[playerPos.x, (playerPos + Vector2Int.down).y] = 1;
				}
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.down).y] == 5) {
				DoSomeAI();
			}
			else if (currentState[playerPos.x, (playerPos + Vector2Int.down).y] == 6) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[playerPos.x, (playerPos + Vector2Int.down + Vector2Int.down).y] == 0) {
					currentState[playerPos.x, (playerPos + Vector2Int.down).y] = 1;
					bots.RemoveAll(bot => bot.pos.x == playerPos.x && (playerPos + Vector2Int.down).y == bot.pos.y);
				}
				else if (currentState[playerPos.x, (playerPos + Vector2Int.down + Vector2Int.down).y] == 1) {
					currentState[playerPos.x, (playerPos + Vector2Int.down + Vector2Int.down).y] = 6;
					currentState[playerPos.x, (playerPos + Vector2Int.down).y] = 1;
				}
			}
			RefreshScene();
		}
		else if (Input.GetKeyDown("a")) {
			if (currentState[(playerPos + Vector2Int.left).x, playerPos.y] == 0) {
				Debug.Log("GAME OVER");
				playerPos += Vector2Int.left;
			}
			else if (currentState[(playerPos + Vector2Int.left).x, playerPos.y] == 1) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;
				}
				currentState[(playerPos + Vector2Int.left).x, playerPos.y] = 2;
				playerPos += Vector2Int.left;
				DoSomeAI();
			}
			else if (currentState[(playerPos + Vector2Int.left).x, playerPos.y] == 3) {
				Debug.Log("ALMOST THERE");
				currentState[playerPos.x, playerPos.y] = 1;
				currentState[(playerPos + Vector2Int.left).x, playerPos.y] = 7;
				playerPos += Vector2Int.left;
				DoSomeAI();
			}
			else if (currentState[(playerPos + Vector2Int.left).x, playerPos.y] == 4) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[(playerPos + Vector2Int.left + Vector2Int.left).x, playerPos.y] == 0) {
					currentState[(playerPos + Vector2Int.left).x, playerPos.y] = 1;
					bots.RemoveAll(bot => bot.pos.x == (playerPos + Vector2Int.left).x && playerPos.y == bot.pos.y);
				}
				else if (currentState[(playerPos + Vector2Int.left + Vector2Int.left).x, playerPos.y] == 1) {
					currentState[(playerPos + Vector2Int.left + Vector2Int.left).x, playerPos.y] = 4;
					bots.Find((bot) => (playerPos + Vector2Int.left).x == bot.pos.x && playerPos.y == bot.pos.y).pos += Vector2Int.left;
					currentState[(playerPos + Vector2Int.left).x, playerPos.y] = 1;
				}
			}
			else if (currentState[(playerPos + Vector2Int.left).x, playerPos.y] == 5) {
				DoSomeAI();
			}
			else if (currentState[(playerPos + Vector2Int.left).x, playerPos.y] == 6) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[(playerPos + Vector2Int.left + Vector2Int.left).x, playerPos.y] == 0) {
					currentState[(playerPos + Vector2Int.left).x, playerPos.y] = 1;
					bots.RemoveAll(bot => bot.pos.x == (playerPos + Vector2Int.left).x && playerPos.y == bot.pos.y);
				}
				else if (currentState[(playerPos + Vector2Int.left + Vector2Int.left).x, playerPos.y] == 1) {
					currentState[(playerPos + Vector2Int.left + Vector2Int.left).x, playerPos.y] = 6;
					currentState[(playerPos + Vector2Int.left).x, playerPos.y] = 1;
				}
			}
			RefreshScene();
		}
		else if (Input.GetKeyDown("d")) {
			if (currentState[(playerPos + Vector2Int.right).x, playerPos.y] == 0) {
				Debug.Log("GAME OVER");
				playerPos += Vector2Int.right;
			}
			else if (currentState[(playerPos + Vector2Int.right).x, playerPos.y] == 1) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;
				}
				currentState[(playerPos + Vector2Int.right).x, playerPos.y] = 2;
				playerPos += Vector2Int.right;
				DoSomeAI();
			}
			else if (currentState[(playerPos + Vector2Int.right).x, playerPos.y] == 3) {
				Debug.Log("ALMOST THERE");
				currentState[playerPos.x, playerPos.y] = 1;
				currentState[(playerPos + Vector2Int.right).x, playerPos.y] = 7;
				playerPos += Vector2Int.right;
				DoSomeAI();
			}
			else if (currentState[(playerPos + Vector2Int.right).x, playerPos.y] == 4) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[(playerPos + Vector2Int.right + Vector2Int.right).x, playerPos.y] == 0) {
					currentState[(playerPos + Vector2Int.right).x, playerPos.y] = 1;
					bots.RemoveAll(bot => bot.pos.x == (playerPos + Vector2Int.right).x && playerPos.y == bot.pos.y);
				}
				else if (currentState[(playerPos + Vector2Int.right + Vector2Int.right).x, playerPos.y] == 1) {
					currentState[(playerPos + Vector2Int.right + Vector2Int.right).x, playerPos.y] = 4;
					bots.Find((bot) => (playerPos + Vector2Int.right).x == bot.pos.x && playerPos.y == bot.pos.y).pos += Vector2Int.right;
					currentState[(playerPos + Vector2Int.right).x, playerPos.y] = 1;
				}
				DoSomeAI();
			}
			else if (currentState[(playerPos + Vector2Int.right).x, playerPos.y] == 5) {
				DoSomeAI();
			}
			else if (currentState[(playerPos + Vector2Int.right).x, playerPos.y] == 6) {
				//dodac mechanizm kolejkowania popchniecia
				if (currentState[(playerPos + Vector2Int.right + Vector2Int.right).x, playerPos.y] == 0) {
					currentState[(playerPos + Vector2Int.right).x, playerPos.y] = 1;
					bots.RemoveAll(bot => bot.pos.x == (playerPos + Vector2Int.right).x && playerPos.y == bot.pos.y);
				}
				else if (currentState[(playerPos + Vector2Int.right + Vector2Int.right).x, playerPos.y] == 1) {
					currentState[(playerPos + Vector2Int.right + Vector2Int.right).x, playerPos.y] = 6;
					currentState[(playerPos + Vector2Int.right).x, playerPos.y] = 1;
				}
			}
			RefreshScene();
		}
		if (currentState[playerPos.x, playerPos.y] == 7 || currentState[playerPos.x, playerPos.y] == 3) {
			Debug.Log("YOU WIN");
		}
	}

	public void DoSomeAI() {
		bots.ForEach((bot) => {
			int distanceX = 0;
			int distanceY = 0;

			if (bot.pos + Vector2Int.up == playerPos) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;	
				}
				playerPos += Vector2Int.up;
				if (currentState[playerPos.x, playerPos.y] == 3) {
					Debug.Log("YOU WIN");
				}
				if (currentState[playerPos.x, playerPos.y] == 0) {
					Debug.Log("GAME OVER");
				} else {
					currentState[playerPos.x, playerPos.y] = 2;
				}
			} else if (bot.pos + Vector2Int.down == playerPos) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;	
				}
				playerPos += Vector2Int.down;
				if (currentState[playerPos.x, playerPos.y] == 3) {
					Debug.Log("YOU WIN");
				}
				if (currentState[playerPos.x, playerPos.y] == 0) {
					Debug.Log("GAME OVER");
				} else {
					currentState[playerPos.x, playerPos.y] = 2;
				}
			} else if (bot.pos + Vector2Int.right == playerPos) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;	
				}
				playerPos += Vector2Int.right;
				if (currentState[playerPos.x, playerPos.y] == 3) {
					Debug.Log("YOU WIN");
				}
				if (currentState[playerPos.x, playerPos.y] == 0) {
					Debug.Log("GAME OVER");
				} else {
					currentState[playerPos.x, playerPos.y] = 2;
				}
			} else if (bot.pos + Vector2Int.left == playerPos) {
				if (currentState[playerPos.x, playerPos.y] == 7) {
					currentState[playerPos.x, playerPos.y] = 3;
				} else {
					currentState[playerPos.x, playerPos.y] = 1;	
				}
				playerPos += Vector2Int.left;
				if (currentState[playerPos.x, playerPos.y] == 3) {
					Debug.Log("YOU WIN");
				}
				if (currentState[playerPos.x, playerPos.y] == 0) {
					Debug.Log("GAME OVER");
				} else {
					currentState[playerPos.x, playerPos.y] = 2;
				}
			} else if (bot.pos.x == playerPos.x) {
				if (currentState[bot.pos.x, bot.pos.y] == 8) {
					currentState[bot.pos.x, bot.pos.y] = 3;
				} else {
					currentState[bot.pos.x, bot.pos.y] = 1;	
				}
				if (bot.pos.y < playerPos.y) {
					bot.pos += Vector2Int.up;
				} else {
					bot.pos += Vector2Int.down;
				}
				if (currentState[bot.pos.x, bot.pos.y] == 3) {
					currentState[bot.pos.x, bot.pos.y] = bot.type + 3;
				} else {
					currentState[bot.pos.x, bot.pos.y] = bot.type;	
				}
			} else if (bot.pos.y == playerPos.y) {
				if (currentState[bot.pos.x, bot.pos.y] == 8) {
					currentState[bot.pos.x, bot.pos.y] = 3;
				} else {
					currentState[bot.pos.x, bot.pos.y] = 1;	
				}
				if (bot.pos.x < playerPos.x) {
					bot.pos += Vector2Int.right;
				} else {
					bot.pos += Vector2Int.left;
				}
				if (currentState[bot.pos.x, bot.pos.y] == 3) {
					currentState[bot.pos.x, bot.pos.y] = 8;
				} else {
					currentState[bot.pos.x, bot.pos.y] = bot.type;	
				}
			} else if (playerPos.x != bot.pos.x && playerPos.y != bot.pos.y) {
				distanceX = Mathf.Abs(playerPos.x - bot.pos.x);
				distanceY = Mathf.Abs(playerPos.y - bot.pos.y);
				if (distanceX >= distanceY && playerPos.x > bot.pos.x) {
					if (currentState[bot.pos.x, bot.pos.y] == 8) {
						currentState[bot.pos.x, bot.pos.y] = 3;
					} else {
						currentState[bot.pos.x, bot.pos.y] = 1;	
					}
					bot.pos += Vector2Int.right;
					if (currentState[bot.pos.x, bot.pos.y] == 3) {
						currentState[bot.pos.x, bot.pos.y] = 8;
					} else {
						currentState[bot.pos.x, bot.pos.y] = bot.type;	
					}
				} else if (distanceX >= distanceY && playerPos.x < bot.pos.x) {
					if (currentState[bot.pos.x, bot.pos.y] == 8) {
						currentState[bot.pos.x, bot.pos.y] = 3;
					} else {
						currentState[bot.pos.x, bot.pos.y] = 1;	
					}
					bot.pos += Vector2Int.left;
					if (currentState[bot.pos.x, bot.pos.y] == 3) {
						currentState[bot.pos.x, bot.pos.y] = 8;
					} else {
						currentState[bot.pos.x, bot.pos.y] = bot.type;	
					}
				} else if (distanceX <= distanceY && playerPos.y > bot.pos.y) {
					if (currentState[bot.pos.x, bot.pos.y] == 8) {
						currentState[bot.pos.x, bot.pos.y] = 3;
					} else {
						currentState[bot.pos.x, bot.pos.y] = 1;	
					}
					bot.pos += Vector2Int.up;
					if (currentState[bot.pos.x, bot.pos.y] == 3) {
						currentState[bot.pos.x, bot.pos.y] = 8;
					} else {
						currentState[bot.pos.x, bot.pos.y] = bot.type;
					}
				} else if (distanceX <= distanceY && playerPos.y < bot.pos.y) {
					if (currentState[bot.pos.x, bot.pos.y] == 8) {
						currentState[bot.pos.x, bot.pos.y] = 3;
					} else {
						currentState[bot.pos.x, bot.pos.y] = 1;	
					}
					bot.pos += Vector2Int.down;
					if (currentState[bot.pos.x, bot.pos.y] == 3) {
						currentState[bot.pos.x, bot.pos.y] = 8;
					} else {
						currentState[bot.pos.x, bot.pos.y] = bot.type;	
					}
				}
			}
		});
	}

	private void RefreshScene() {
		Debug.Log("p " + playerPos);
		bots.ForEach((bot) => {
			Debug.Log("b " + bot.pos);
		});
		int childs = objHold.childCount;
		for (int i = childs - 1; i > 0; i--) {
		    GameObject.Destroy(objHold.GetChild(i).gameObject);
		}

		for (int i = 0; i < 8; ++i) {
			for (int j = 0; j < 8; ++j) {
				switch (currentState[i, j]) {
					case 0 : break;
					case 1 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); break;}
					case 2 : {
						Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold);
						Instantiate(player, new Vector3(i, 0, j), player.transform.rotation, objHold);
						break;
					}
					case 3 : {Instantiate(exit, new Vector3(i, 0, j), exit.transform.rotation, objHold); break;}
					case 4 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); Instantiate(enemy1, new Vector3(i, 0, j), enemy1.transform.rotation, objHold); break;}
					case 5 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); Instantiate(enemy2, new Vector3(i, 0, j), enemy2.transform.rotation, objHold); break;}
					case 6 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); Instantiate(enemy3, new Vector3(i, 0, j), enemy3.transform.rotation, objHold); break;}
					case 7 : {Instantiate(exit, new Vector3(i, 0, j), exit.transform.rotation, objHold); Instantiate(player, new Vector3(i, 0, j), player.transform.rotation, objHold); break;}
					case 8 : {Instantiate(exit, new Vector3(i, 0, j), exit.transform.rotation, objHold); Instantiate(enemy1, new Vector3(i, 0, j), enemy1.transform.rotation, objHold); break;}
				}
			}
		}
	}

	//[Button("Generate")]
	public void Generate() {
		for (int i = 0; i < 8; ++i) {
			for (int j = 0; j < 8; ++j) {

				currentState[i, j] = layerPattern[i, j];

				switch (layerPattern[i, j]) {
					case 0 : break;
					case 1 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); break;}
					case 2 : {
						Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold);
						Instantiate(player, new Vector3(i, 0, j), player.transform.rotation, objHold);
						playerPos = new Vector2Int(i, j);
						break;
					}
					case 3 : {Instantiate(exit, new Vector3(i, 0, j), exit.transform.rotation, objHold); break;}
					case 4 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); Instantiate(enemy1, new Vector3(i, 0, j), enemy1.transform.rotation, objHold); break;}
					case 5 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); Instantiate(enemy2, new Vector3(i, 0, j), enemy2.transform.rotation, objHold); break;}
					case 6 : {Instantiate(tile, new Vector3(i, 0, j), tile.transform.rotation, objHold); Instantiate(enemy3, new Vector3(i, 0, j), enemy3.transform.rotation, objHold); break;}
				}
			}
		}
	}


}
