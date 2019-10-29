import React from 'react';
import styles from './index.css';
import {containerName, getSaasToken} from "../../../helpers";

export class Image extends React.Component {
  render() {
    if(typeof(this.props.columnValue.value) === "string") {
      return <span>{this.props.columnValue.value}</span>;
    }

    return <a target="_blank" href={this.props.columnValue.value.url+'?'+getSaasToken(containerName)}>
      <img className={styles.image}
           src={this.props.columnValue.value.url}
           alt={this.props.columnValue.value.name}
      />
    </a>;
  }
}
