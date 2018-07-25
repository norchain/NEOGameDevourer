using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Helper = Neo.SmartContract.Framework.Helper;
using System.Text;

using System.ComponentModel;
using System.Numerics;
using System.Collections.Generic;


namespace Devourer
{
	public class Devourer: SmartContract
    {

		public static class Const{
			public const int blocksPerMove = 5;
			public const int limitPlayer = 30;

		}

		public static class Color{
			public const byte Yellow = 0;
			public const byte Purple = 1;
			public const byte White = 2;
			public const byte Green = 3;
		}



		public class Pos{
			public byte x;
			public byte y;

		}

		public class Cell{
			public Pos pos;
			public byte type;
		}
      

		public class User{
			public BigInteger warID;
			public byte color;
			public Cell[] cells;
			public byte[] address;
			public BigInteger score;
		}

		public class War{
			public BigInteger[] users;
			public byte[] awardFactor; 
		}


        


		public static byte[] Serialize(Pos pos)
        {
			//[TODO]
            return new byte[10];

        }

		public static byte[] Serialize(Cell cell)
        {
			//[TODO]
            return new byte[10];

        }

		public static byte[] Serialize(User user)
        {
			//[TODO]
            return new byte[10];

        }

		public static byte[] Serialize(War war)
        {
			//[TODO]
            return new byte[10];

        }

  
		public static Pos DeserializeToPos(byte[] data)
        {
			Pos pos = new Pos();
			pos.x = data[0];
			pos.y = data[1];
            return pos;
        }
  

		public static Cell DeserializeToCell(byte[] data)
        {
			//[TODO]
			Cell cell = new Cell();
			cell.pos = DeserializeToPos(data);
			return cell;
        }


		public static User DeserializeToUser(byte[] data)
        {
			//[TODO]
			User user = new User();
			user.address = "1010101".HexToBytes();
			return user;
        }

		public static War DeserializeToQuestion(byte[] data)
        {
			//[TODO]
			War war = new War();
			war.awardFactor = data;
			return war;
        }

        


		private static User[] FindAllUsers(War war){
			BigInteger[] userIds = war.users;
			User[] users = new User[userIds.Length];
			for (int i = 0; i < userIds.Length; i++){
				User user = GetUser(war,userIds[i]);
				users[i] = user;
			}
			return users;
		}


		private static bool IsUserInWar(War war, byte[] address)
        {
            User[] users = FindAllUsers(war);
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i].address == address)
                {
                    return true;
                }
            }
            return false;
        }

		private static bool IsUserInWar(War war, User user){
			return IsUserInWar(war, user.address);
		}

		private static Cell[] FindAllCells(War war){
			User[] users = FindAllUsers(war);
			int num = 0;
			for (int i = 0; i < users.Length; i ++){
				num = num + users[i].cells.Length;
			}
            

            
			Cell[] cells = new Cell[num];
			int k = 0;
			for (int i = 0; i < users.Length; i++)
            {
				User user = users[i];
				for (int j = 0; j < user.cells.Length; i++){
					Cell cell = user.cells[j];
					cells[k] = cell;
					++k;
				}

            }
			return cells;
            
		}
        
		private static Cell[] FindCellsAround (War war, Pos pos){
			//TODO
			Cell[] cells = FindAllCells(war);
			Cell[] ret = new Cell[cells.Length];
			int j = 0;
			for (int i = 0; i < cells.Length; i++){
				Cell cell = cells[i];
				if (cell.pos.x == pos.x -1 || cell.pos.x == pos.x + 1 ){
					if(cell.pos.y == pos.y - 1 || cell.pos.y == pos.y){
						ret[j] = cell;
						++j;
					}
				}
			}
			return ret;
		}



		private static bool Register (BigInteger warID,byte[] address, byte color){
			War war = GetWar(warID);

			if(IsUserInWar(war,address)){
				return false;
			}
			else{
				if(war.users.Length >= Const.limitPlayer){
					return false;
				}
				else{
					User user = new User();
                    user.address = address;
                    user.color = color;

					war.users[war.users.Length] = new BigInteger(user.address) ;
					return true;
				}

			}

            
		}

		private static BigInteger Move(BigInteger warID, byte[] address,byte[] positions){
			//TODO
			return 0;
		}
        
		private static BigInteger AwardSurvive (User user){
			//TODO
            return 1000;
		}
        
		private static BigInteger AwardKill(Cell killed){
			//TODO
            return 1000;
		}

		private static BigInteger AwardAbsorb(Cell cell){
			//TODO
			return 1000;
		}


       
        
		public static object Main (string op, params object[] args){
			if (Runtime.Trigger == TriggerType.Application)
            {
                if (op == "register")
                {
					BigInteger warID = (BigInteger)args[0];
					byte[] address = (byte[])args[1];
					byte color = ((byte[])args[2])[0];
					return Register(warID,address, color);
                }
				if( op == "move"){
					BigInteger warID = (BigInteger)args[0];
					byte[] address = (byte[])args[1];
					byte[] positions = (byte[])args[2];
					return Move(warID, address, positions);
				}
				return false;
            }
			else{
				return false;
			}
		}


		public static War GetWar(BigInteger warId)
        {
			//[TODO]
			return new War();
        }

		public static User GetUser(War war, BigInteger userId){
			//[TODO]
			return new User();
		}

		public static Cell GetCell(Pos pos){
			//[TODO]
			return new Cell();
		}
    }
}
