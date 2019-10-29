import React from 'react';
import {sidebar, links, icon, collapsed, link, activeLink, logo, shortLogo, logoContainer, back, backArrow} from './index.css';
import {Icon} from '../icon';
import {Link} from 'react-router';
import {idFor} from '../../helpers';
import {browserHistory} from "react-router";
import classNames from 'classnames';
import {button} from '../../css-common/forms.css';

export class SidebarForBack extends React.Component {
  constructor(props) {
    super();
    this.state = {collapsed: !!props.collapsed};
  }

  render() {
    const classes = classNames(button, back);
    return (<div id={idFor('sidebar')}>
      <Link className={logoContainer} to="/">
        <Icon name="squirrel" className={shortLogo}/>
      </Link>
        <button className={classes} onClick={browserHistory.goBack}>
          <Icon name="left" className={backArrow}/>
        </button>
    </div>);
  }
}
