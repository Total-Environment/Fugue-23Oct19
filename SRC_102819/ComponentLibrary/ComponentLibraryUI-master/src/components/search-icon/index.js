import React, { Component } from 'react';
import { Icon } from '../icon';
import {icon} from './index.css';
import classNames from 'classnames';

export class SearchIcon extends Component {
    render() {
        return <Icon name="search" className={classNames(icon, this.props.className)} />;
    }
}