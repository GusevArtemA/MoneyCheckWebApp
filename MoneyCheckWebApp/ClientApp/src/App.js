import React, { Component } from 'react';
import {Redirect, Route, Switch} from 'react-router-dom';
import {Login} from "./components/Login";
import {Layout} from "./components/Layout";
import {Welcome} from "./components/Welcome";
import {Home} from "./components/Home";
import "./App.css";
import {AnalyticsPage} from "./components/AnalyticsPage";
import {InflationPage} from "./components/InflationPage";
import {ManageAccountComponent} from "./components/ManageAccountComponent";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Layout>
            <Route path={['/home', '/analytics', '/inflation']} component={ManageAccountComponent}/>
            <Switch>
                <Route exact path='/login' component={Login} />
                <Route exact path='/welcome' component={Welcome} />
                <Route exact path='/home' component={Home} />
                <Route exact path='/analytics' component={AnalyticsPage}/>
                <Route exact path='/inflation' component={InflationPage}/>
                <Route>
                    <Redirect to='/welcome'/>
                </Route>    
            </Switch>
        </Layout>
    );
  }
}
