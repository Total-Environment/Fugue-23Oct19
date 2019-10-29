import React from 'react';
import styles from './index.css';
import classNames from 'classnames';
import {AddDocumentConnector} from '../../../add_document_connector'
import * as R from 'ramda';
import {containerName, getSaasToken} from "../../../helpers";

export class
StaticFile extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      addFile: {
        message: '',
        shown: false
      }
    };
    this.showAddFileDialog = this.showAddFileDialog.bind(this);
    this.handleCancel = this.handleCancel.bind(this);
    this.handleOk = this.handleOk.bind(this);
  }

  renderShowRemove(hideRemove) {
    if (!hideRemove) {
      return (<button className={styles.delete} onClick={(e) => {
        e.preventDefault();
        this.setState({addFile: {shown: false}});
        this.props.onChange(null);
      }}>×
      </button>);
    }
  }

  handleOk(e) {
    this.setState({addFile: {shown: false}});
    this.props.onChange(e.document);
  }

  handleCancel() {
    this.setState({addFile: {shown: false}});
  }

  showAddFileDialog() {
    this.setState({addFile: {shown: true}});
  }

  render() {
    if (this.props.columnValue.editable === true) {
      return (<div>
        {this.renderFile()}
      </div>);
    }

    if (typeof(this.props.columnValue.value) === "string") {
      return <span>{this.props.columnValue.value}</span>;
    }
    return <a className={styles.link} target="_blank"
              href={this.props.columnValue.value.url+'?'+getSaasToken(containerName)}>{this.props.columnValue.value.name}</a>;
  }

  renderLink() {
    if (this.props.columnValue.value.url) {
      return (<div className={styles.singleData}>
        <a className={styles.link} target="_blank"
           href={this.props.columnValue.value.url+'?'+getSaasToken(containerName)}>{this.props.columnValue.value.name}</a>
      </div>);
    }
    else {
      return (<div className={styles.singleData}>
        {window.fileList[this.props.columnValue.value][0].name}
      </div>);
    }
  }

  renderFile() {
    let columnName = this.props.columnValue.key;
    if (this.props.columnValue && !R.isNil(this.props.columnValue.value) && !R.isEmpty(this.props.columnValue.value)) {
      return (<div>
        <div className={styles.row}>
          {this.renderLink()}
          {this.renderShowRemove(this.props.hideRemove)}
        </div>
      </div>);
    }
    else {
      return (<div>
        <div className={styles.row}>
          <button className={styles.addFile} onClick={(e) => {
            e.preventDefault();
            (() => this.showAddFileDialog())();
          }}>Add File
          </button>
          {this.renderShowRemove(this.props.hideRemove)}
        </div>
        {this.state.addFile.shown &&
        <AddDocumentConnector group={this.props.group} columnName={columnName}
                              columnHeader={(this.props.columnValue && this.props.columnValue.name) || this.props.columnName}
                              componentType={this.props.componentType}
                              showLocalSystem={true}
                              onOk={this.handleOk}
                              onCancel={this.handleCancel}/>
        }
      </div>);
    }
  }
}
