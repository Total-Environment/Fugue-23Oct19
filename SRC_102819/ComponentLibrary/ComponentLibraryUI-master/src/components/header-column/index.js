import React from 'react';
import Collapse from 'rc-collapse';
import {Column} from "../column";
import {Icon} from "../icon/index";
import styles from './index.css';
import flexboxgrid from 'flexboxgrid/css/flexboxgrid.css';
import classNames from 'classnames';

export class HeaderColumn extends React.Component {
  constructor(props) {
    super(props);
    this.renderHeader = this.renderHeader.bind(this);
    this.renderColumn = this.renderColumn.bind(this);
  }

  renderColumn(column, headerKey, index) {
    return <div className={styles.columnContainer} key={`${headerKey}_${index}`}>
      <Column
        group={this.props.group}
        componentType={this.props.componentType}
        details={column}
        onChange={(value) => this.props.onChange(headerKey, column.key, value, column.name)}/>
    </div>;
  }

  renderHeader(header) {
    return <Collapse.Panel
      key={header.key}
      showArrow={false}
      header={<span id={header.key}>{this.props.showArrow &&
      <Icon name="arrow" className="arrow"/>}{header.name}</span>}>
      <div className={classNames(flexboxgrid.row, styles.header)}>
        {header.columns.map((column, i) => this.renderColumn(column, header.key, i))}
      </div>
    </Collapse.Panel>;
  }

  render() {
    const props = this.props.activeHeaders ? {activeKey: this.props.activeHeaders} : {};
    return <div className={styles.headerColumn}><Collapse {...props}>
      {this.props.headers.map(this.renderHeader)}
    </Collapse></div>;
  }
}
HeaderColumn.defaultProps = {
  showArrow: true,
};
