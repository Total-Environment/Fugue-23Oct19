import React from 'react';
import DataType from '../../data-types';
import {Image} from '../../data-types/image';
import {ReadMore} from '../../read-more';
import styles from './index.css';
import classNames from 'classnames';

export class Array extends React.Component {
  constructor(props) {
    super(props);
    this.handleAddAnotherClick = this.handleAddAnotherClick.bind(this);
    this.handleChange = this.handleChange.bind(this);
  }

  handleChange(value, index) {
    const clonedValue = JSON.parse(JSON.stringify(this.props.columnValue.value));
    clonedValue[index] = value;
    this.props.onChange(clonedValue);
  }

  handleDelete(index) {
    const newValues = this.props.columnValue.value.filter((e, i) => i !== index);
    this.props.onChange(newValues.length === 0 ? null : newValues);
  }

  renderInputValue(columnValue, index) {
    return <div key={index} className={styles.singleData}>
      <div className={styles.dataType}>
      <DataType columnName={this.props.columnName} columnValue={columnValue}
                group={this.props.group} componentType={this.props.componentType.toLowerCase()}
                hideRemove
                onChange={value => this.handleChange(value, index)} />
      </div>
      <button className={styles.delete} onClick={(e) => {
        e.preventDefault();
        this.handleDelete(index);
      }}>-
      </button>
    </div>;
  }

  renderValue(columnValue, index) {
    // if (index === 0 && columnValue.value && !columnValue.editable) {
    //   const isImage = (/\.(gif|jpg|jpeg|tiff|png)$/i).test(columnValue.value.url);
    //   if (isImage) {
    //     return <div key={index}><Image columnName={this.props.columnName} columnValue={columnValue}/></div>;
    //   }
    // }
    return columnValue.editable ? this.renderInputValue(columnValue, index) : this.renderDataValue(columnValue, index);
  }

  renderDataValue(columnValue, index) {
    return <div className={styles.singleData} key={index}>
      <DataType columnName={this.props.columnName} columnValue={columnValue} group={this.props.group} componentType={this.props.componentType.toLowerCase()}/>
    </div>;
  }

  handleAddAnotherClick(e) {
    e.preventDefault();
    this.props.onChange((this.props.columnValue.value || []).concat([null]));
  }

  renderInput(children) {
    return <div>
      {children}
      <button className={styles.addAnother} onClick={this.handleAddAnotherClick}>+
        Add {this.props.columnValue.value ? 'Another' : ''}</button>
    </div>
  }

  render() {
    if(this.props.columnValue.editable && this.props.columnValue.filterable) {
      const columnValue = Object.assign({}, this.props.columnValue, {dataType: this.props.columnValue.dataType.subType});
      return <DataType columnName={this.props.columnName}
                       columnValue={columnValue}
                       group={this.props.group}
                       componentType={this.props.componentType.toLowerCase()}
                       onChange={this.props.onChange}/>
    }
    const columnValue = Object.assign({}, this.props.columnValue, {dataType: this.props.columnValue.dataType.subType});
    const values = this.props.columnValue.value || []; // This should be null only if editable is true
    const children = values.map((value, index) => this.renderValue(Object.assign({}, columnValue, {value, validity: {isValid: true, msg: ''}}), index));
    return this.props.columnValue.editable ? this.renderInput(children) : <ReadMore length="2">{children}</ReadMore>;
  }
}
