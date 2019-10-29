import React from 'react';
import {Topbar} from '../components/topbar';
import {Sidebar} from '../components/sidebar';
import {app, sidebar, topbar, content, footer, shortLogo} from './index.css';
import {FullLoadingConnector} from "../full_loading_connector";

export class App extends React.Component {
  render() {
    return (<div className={app}>
      <Sidebar className={sidebar} collapsed={!!this.props.children}/>
      <Topbar className={topbar}/>
      <div className={content}>
        {this.props.children}
      </div>
      <footer className={footer}>&copy; Total Environment</footer>
      <FullLoadingConnector />
    </div>);
  }
}
