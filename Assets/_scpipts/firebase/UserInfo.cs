using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo  {

    public string uid;
    public string full_name;
    public string share_code;
    public int curent_mode=1;//CURRENT_MODE
    public int puzzle_no;//PUZZLE_NO
    public int coin;//COIN
    public int old_puzzle_id;//OLD_PUZZLE_ID
    public string hint_char_indexs="";//HINT_CHAR_INDEXS
    public bool entered_code = false; 
    public List<string> success_shared_queue=new List<string>();
}
