import React from 'react';

import { extraInformationContent, sidebar, content, footer, shortLogo, back} from './index.css';
import {SidebarForBack} from "../sidebar-with-back/index";



export class ExtraInformationContent extends React.Component {
  render() {

    return (<div className={extraInformationContent}>
      <SidebarForBack className={sidebar}/>
      <div className={content}>
        {this.props.children}
      </div>

      <footer className={footer}>&copy; Total Environment</footer>
    </div>);
  }
}
