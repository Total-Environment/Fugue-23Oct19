import React, { Component } from 'react';
import { Icon } from '../icon';
import {container, icon} from './index.css';
import { idFor } from '../../helpers';
import classNames from 'classnames';

export class Loading extends Component {
  render() {
    return <div id={idFor("Loading")} className={container}><Icon name="loading" className={classNames(icon, this.props.className)} /></div>;
  }
}
