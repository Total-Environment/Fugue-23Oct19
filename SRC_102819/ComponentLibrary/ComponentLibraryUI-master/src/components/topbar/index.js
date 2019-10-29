import React, {Component} from 'react';
import styles, {topbar, left, right, logo} from './index.css';
import classNames from 'classnames';
import DropdownMenu from "react-dd-menu";
import {Icon} from "../icon/index";
import {logInfo} from "../../sumologic-logger";

export class Topbar extends Component {
  constructor() {
    super();
    this.state = {
      isMenuOpen: false
    };

    this.click = this.click.bind(this);
    this.toggle = this.toggle.bind(this);
    this.close = this.close.bind(this);
  }

  toggle() {
    this.setState({ isMenuOpen: !this.state.isMenuOpen });
  }

  close() {
    this.setState({ isMenuOpen: false });
  }

  click(e) {
    e.preventDefault();
    logInfo('User ' + window.userInformation.userName + '(' + window.userInformation.email + ')' + 'logged out from CL');
    window.adalInstance.logOut();
  }

  render() {
    const menuOptions = {
      isOpen: this.state.isMenuOpen,
      close: this.close,
      toggle: <div>
        <button type="button" className={styles.button} onClick={this.toggle}>
          <span>Welcome, <b>{window.userInformation && window.userInformation.userName}</b></span>
        </button>
      </div>,
      align: 'right',
      animAlign: 'right',
      size: 'sm'
    };
    return (<div className={classNames(topbar, this.props.className)}>
      <DropdownMenu {...menuOptions}>
        <div className={styles.dropdown}>
        <li><button type="button" onClick={this.click} className={styles.logout}> <Icon name="logout" className={styles.icon}/> Logout</button></li>
        </div>
      </DropdownMenu>
    </div>);
  }
}
