import React from 'react';
import {Tab, Tabs, TabList, TabPanel} from 'react-tabs';
import styles from './index.css';
import {idFor} from "../../helpers";
import DataType from "../data-types/index";


export class FilterDataComponent extends React.Component {
  constructor(props) {
    super(props);
    this.state = {selectedHeader: 'Classification'}
  }
  render() {
    const headerNames = this.props.definition && this.props.definition.headers.map(header => header.name);
    const headerKeys = this.props.definition && this.props.definition.headers.map(header => header.key);
    const selectedTab = headerKeys.indexOf(this.state.selectedHeader);
    return (<Tabs selectedIndex={selectedTab} onSelect={i => this.onTabChange(headerKeys[i])}>
      <TabList>
        {headerNames.map(h => <Tab>{h}</Tab>)}
      </TabList>
      {this.renderData(this.props.definition.headers)}
    </Tabs>);
  }

  onTabChange(header) {
    this.setState({selectedHeader:header});
  }

  renderData(headers) {
    return headers.map(header => this.renderHeader(header.key,header.columns));
  }

  renderHeader(headerKey,columns) {
    if (headerKey === "classification") return this.renderClassification(columns,headerKey);
    return <TabPanel><div className={styles.header}>{columns.map(column => this.renderColumn(headerKey,column))}</div></TabPanel>;
  }

  renderClassification(columns,headerKey) {
    const newColumns = JSON.parse(JSON.stringify(columns));
    const columnsArray = newColumns.map(column => ({ value: column, name: column.name }));
    if (this.props.dependencyDefinition) {

      let number = this.getFirstModifiableLevelNumber();
      let children = this.getChildrenForClassificationLevels(number,this.props.dependencyDefinition.values.block.children, this.props.levels[0]);
      this.setValues(columnsArray, children);
    }
    return <TabPanel><div className={styles.header}>{newColumns.map(column => this.renderColumn(headerKey,column))}</div></TabPanel>;
  }

  getChildrenForClassificationLevels(levelNumber, children, levelValue) {
    levelNumber = levelNumber - 1;
    if(!levelNumber) { return children;}
    return this.getChildrenForClassificationLevels(levelNumber,children.find(c => c.name === levelValue).children, this.props.levels[1]);
  }

  getFirstModifiableLevelNumber() {
    switch(this.props.componentType) {
      case 'material':
        return 3;
      case 'service':
        return 2;
      default:
        return 1;
    }
  }

  setValues(columns, children) {
    const currentColumn = columns[0];
    if (!currentColumn) return;
    currentColumn.value.dataType.values = {
      values: children.map(child => child.name).filter(name => name !== "null"),
      status: 'fetched'
    };
    if (!currentColumn.value.value) {
      columns.filter((c, i) => i !== 0).forEach(column => {
        column.value.value = '';
        if (column.value.dataType.values) {
          column.value.dataType.values.values = [];
        }
      });
    }
    const remainingDependencyDefinition = children.find(child => child.name === currentColumn.value.value);
    if (!remainingDependencyDefinition) {
      columns.filter((c, i) => i !== 0).forEach(column => {
        column.value.value = '';
        if (column.value.dataType.values) {
          column.value.dataType.values.values = [];
        }
      });
      return;
    }
    this.setValues(columns
      .filter((c, i) => i !== 0), remainingDependencyDefinition.children);
  }

  renderColumn(headerKey, column) {
    column.isRequired = false;
    return (<div className={styles.column}>
      <dt id={idFor(column.key, 'title')} className={styles.columnTitle}>{column.name}</dt>
      <dd id={idFor(column.key, 'value')} className={styles.columnValue}>
        <DataType columnName={column.key}
                  columnValue={column}
                  group={this.props.group}
                  componentType={this.props.componentType.toLowerCase()}
                  onChange={(value) => this.props.onColumnChange(headerKey,column.key,value)}
                  isClassification={column.isClassification}
        />
      </dd>
    </div>);
  }
}
