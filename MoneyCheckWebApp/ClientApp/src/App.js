import React, { Component } from 'react';
import {Route} from 'react-router-dom';
import {Login} from "./components/Login";
import {Layout} from "./components/Layout";
import {Welcome} from "./components/Welcome";
import {Home} from "./components/Home";
import "./App.css";
import {AnalyticsPage} from "./components/AnalyticsPage";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Layout>
            <Route exact path='/login' component={Login} />
            <Route exact path='/welcome' component={Welcome} />
            <Route exact path='/home' component={Home} />
            <Route exact path='/analytics' component={AnalyticsPage}/>
        </Layout>
    );
  }
}
