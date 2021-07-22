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
    getAccessToken({ username: "admin", password: "admin" }).then(data => {
      console.log("Page Success: ", data);
      setToken("Access_Token", data.jwtToken);
      setResultInDiv(data, "Access Token:");
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.", "Access Token:");
    });
  }
  function clickRefresh() {
    getRefreshToken().then(data => {
      console.log("Page Success: ", data);
      setToken("Access_Token", data.jwtToken);
      setResultInDiv(data, "Refresh Token:");
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.", "Refresh Token:");
    });
  }
  function clickAccessApplication() {
    getResult().then(data => {
      console.log("Page Success: ", data);
      setResultInDiv(data, "Access Application:");
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.", "Access Application:");
    });
  }

  function fnClearRefreshToken() {
    revokeRefreshToken().then(data => {
      console.log("Page Success: ", data);
      setResultInDiv(data, "Revoke Token:");
    }).catch(err => {
      console.log("Page Error: ", err);
      setResultInDiv("Error Occured.", "Revoke Token:");
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
  function setResultInDiv(obj, msg) { setResult(msg + " " + JSON.stringify(obj)); }
  return (
    <div>
      <div className="form-group">
        <label for="exampleInputEmail1">Local Access Token</label>
        <input type="email" class="form-control" id="exampleInputEmail1" aria-describedby="emailHelp" placeholder="Enter local token"></input>
      </div>
      <div className="btn btn-group" style={{ paddingLeft: "0px" }}>
        <button onClick={clickAccessToken} type="button" className="btn btn-info">Gen. Access Token</button>
        <button onClick={clickRefresh} type="button" className="btn btn-info">Gen. Refresh Token</button>
        <button onClick={clickAccessApplication} type="button" className="btn btn-success">Access Application</button>
        <button onClick={fnClearAccessToken} type="button" className="btn btn-warning">Clear Access</button>
        <button onClick={fnClearRefreshToken} type="button" className="btn btn-warning">Clear Refresh</button>
        <button onClick={fnSetAccessToken} type="button" className="btn btn-info">Set Local as Access</button>
      </div>
      <hr style={{ float: "left", width: "1000px" }} />
      <div className="card" style={{ width: "1000px" }}>
        <div className="card-body">
          <h5 className="card-title">Result</h5>
          <p className="card-text">{result}</p>
        </div>
      </div>
    </div>
  );
}
