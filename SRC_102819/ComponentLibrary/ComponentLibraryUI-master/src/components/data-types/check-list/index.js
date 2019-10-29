import React from 'react';
import styles from './index.css';
import { AddDocumentConnector } from '../../../add_document_connector'

export class CheckList extends React.Component {
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

  renderLink() {
    return (<div className={styles.singleData}>
      <a className={styles.link} target="_blank"
        href={`/check-lists/${this.props.columnValue.value.id}`}>{this.props.columnValue.value.id}</a>
    </div>);
  }

  renderShowRemove(hideRemove) {
    if (!hideRemove) {
      return (<button className={styles.delete} onClick={(e) => {
        e.preventDefault();
        this.setState({ addFile: { shown: false } });
        this.props.onChange(null);
      }}>×
      </button>);
    }
  }

  showAddFileDialog() {
    this.setState({ addFile: { shown: true } });
  }

  handleOk(e) {
    this.setState({ addFile: { shown: false } });
    this.props.onChange(e.document);
  }

  handleCancel() {
    this.setState({ addFile: { shown: false } });
  }

  renderFile() {
    let columnName = this.props.columnValue.key;
    if (this.props.columnValue && this.props.columnValue.value) {
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
            showLocalSystem={false} onOk={this.handleOk}
            onCancel={this.handleCancel} />
        }
      </div>);
    }
  }

  render() {
    if (this.props.columnValue.editable) {
      return (<div>
        {this.renderFile()}
      </div>);
    }
    if (typeof (this.props.columnValue.value) === "string") {
      return <span>{this.props.columnValue.value}</span>;
    }
    return <a className={styles.link} target="_blank"
      href={`/check-lists/${this.props.columnValue.value.id}`}>{this.props.columnValue.value.id}</a>;
  }
}
