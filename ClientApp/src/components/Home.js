import React, { useState, useEffect } from 'react';
import { getResult, getRefreshToken, getAccessToken, revokeRefreshToken } from '../services/homeService';
import { setToken } from '../helpers/localStorage';

//export class Home extends Component {
export function Home() {
  const [result, setResult] = useState("Unknown");
  const [localToken, setlocalToken] = useState("");
  useEffect(() => { setToken("Access_Token", null); }, [])
  //useEffect(() => { }, [result])

  function clickAccessToken() {
    alert("Access Token");
    getAccessToken({ username: "admin", password: "admin" }).then(data => {
      console.log("Page Success: ", data);
      setToken("Access_Token", data.jwtToken);
      setResultInDiv(data);
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.");
    });
  }
  function clickRefresh() {
    alert("Refresh Token");
    getRefreshToken().then(data => {
      console.log("Page Success: ", data);
      setToken("Access_Token", data.jwtToken);
      setResultInDiv(data);
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.");
    });
  }
  function clickAccessApplication() {
    alert("Access Application");
    getResult().then(data => {
      console.log("Page Success: ", data);
      setResultInDiv(data);
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.");
    });
  }

  function fnClearRefreshToken() {
    alert("Revoke Refresh Token");
    revokeRefreshToken().then(data => {
      console.log("Page Success: ", data);
      setResultInDiv(data);
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.");
    });
  }


  function fnSetAccessToken() {
    setToken("Access_Token", localToken);
    setResultInDiv("Access Token Creation Done.");
  }
  function fnClearAccessToken() {
    setToken("Access_Token", "");
    setResultInDiv("Access Token Clear Done.");
  }
  const onCodeChange = (e) => { setlocalToken(e.target.value); };
  function setResultInDiv(obj) { setResult(JSON.stringify(obj)); }
  return (
    <div style={{ width: "1000px" }}>
      <div className="boxOuter" style={{ width: "300px", float: "left" }}>
        <div style={{ padding: "5px", height: "50px" }}>
          Accessing Tokens
        </div>
        <div style={{ padding: "5px" }}>
          <button onClick={clickAccessToken}>Click Me to Generate Access Token</button>
        </div>
        <div style={{ padding: "5px" }}>
          <button onClick={clickRefresh}>Click Me to Refresh the Access Token</button>
        </div>
        <div style={{ padding: "5px" }}>
          <button onClick={clickAccessApplication}>Click Me to Access Application</button>
        </div>
      </div>
      <div className="boxOuter" style={{ width: "700px", float: "right" }}>
        <div style={{ padding: "5px", height: "50px" }}>
          <input style={{ width: "100%" }} type="text" placeholder={"Local Token"} value={localToken} onChange={onCodeChange} />
        </div>
        <div style={{ padding: "5px" }}>
          <button onClick={fnSetAccessToken}>Click Me to Set Access Token</button>
        </div>
        <div style={{ padding: "5px" }}>
          <button onClick={fnClearAccessToken}>Click Me to Clear Access Token</button>
        </div>
        <div style={{ padding: "5px" }}>
          <button onClick={fnClearRefreshToken}>Click Me to Clear Refresh Token</button>
        </div>
      </div>
      <hr style={{ float: "left", width: "1000px" }} />
      <div className="boxOuter" style={{ width: "1000px", float: "left", margin: "0px 0px 30px 0px" }}>
        <div style={{ fontSize: "18px", fontWeight: "bold" }}>Result</div>
        <div style={{ width: "500px", wordWrap: "break-word" }}>{result}</div>
      </div>
    </div>
  );
}
