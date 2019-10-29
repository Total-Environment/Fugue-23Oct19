import React from "react";
import {Table, __RewireAPI__ as TableRewired} from "../../../src/components/table/index";
import {shallow} from "enzyme";
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import * as sinon from "sinon";
import sinonChai from 'sinon-chai';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Table', () => {
  describe('render', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        headers: [
          {name: 'Material Level 1', type: 'text', key: 'material_level_1', sortKey: 'material_level_1_sort', sortable: true},
          {name: 'Rate', type: 'number', key: 'rate', sortKey: 'rate_sort', sortable: true},
          {name: 'Description', key: 'description', sortKey: 'description_sort'},
        ],
        data: [
          {'material_level_1': 'Primary', 'rate': 30, 'description': 'Primary material'},
          {'material_level_1': 'Secondary', 'rate': 40, 'description': 'Secondary machines'},
        ],
      };
      TableRewired.__Rewire__('styles', {table: 'table', number: 'number', text: 'text'});
      wrapper = shallow(<Table {...props} />);
    });
    it('should render table with proper className', () => {
      wrapper = shallow(<Table {...props} />);
      expect(wrapper.find('table')).to.have.className('table');
    });

    it('should render table with the className that is passed as props', () => {
      props.className = 'striped';
      wrapper = shallow(<Table {...props} />);
      expect(wrapper.find('table')).to.have.className('striped');
    });

    it('should show header with passed headers', () => {
      expect(wrapper.find('th').first()).to.include.text('Material Level 1');
      expect(wrapper.find('th').at(1)).to.include.text('Rate');
      expect(wrapper.find('th').at(2)).to.include.text('Description');
    });

    it('should show data in rows', () => {
      const firstRow = wrapper.find('tr').at(1);
      expect(firstRow.find('td').first()).to.include.text('Primary');
      expect(firstRow.find('td').at(1)).to.include.text(30);
      expect(firstRow.find('td').at(2)).to.include.text('Primary material');

      const secondRow = wrapper.find('tr').at(2);
      expect(secondRow.find('td').first()).to.include.text('Secondary');
      expect(secondRow.find('td').at(1)).to.include.text(40);
      expect(secondRow.find('td').at(2)).to.include.text('Secondary machines');
    });

    it('should render column with text style if type is text', () => {
      expect(wrapper.find('th').at(0)).to.have.className('text');
      expect(wrapper.find('tr').at(1).find('td').at(0)).to.have.className('text');
    });
    it('should render column with number style if type is number', () => {
      expect(wrapper.find('th').at(1)).to.have.className('number');
      expect(wrapper.find('tr').at(1).find('td').at(1)).to.have.className('number');
    });
    it('should render column with text style if type is not defined', () => {
      expect(wrapper.find('th').at(2)).to.have.className('text');
      expect(wrapper.find('tr').at(1).find('td').at(2)).to.have.className('text');
    });

    it('should not show Pagination when showPagination is false', () => {
      props.showPagination = false;
      wrapper = shallow(<Table {...props} />);
      expect(wrapper).to.not.have.descendants('Pagination');
    });

    context('when pagination is enabled', () => {
      beforeEach(() => {
        props.showPagination = true;
        props.pageNumber = 1;
        props.totalRecords = 24;
        props.batchSize = 20;
        props.onPageChange = sinon.spy();
        wrapper = shallow(<Table {...props} />);
      });
      it('should have Pagination', () => {
        const pagination = wrapper.find('Pagination');
        expect(pagination).to.have.length(1);
        expect(pagination).to.have.prop('current', 1);
        expect(pagination).to.have.prop('total', 24);
        expect(pagination).to.have.prop('pageSize', 20);
      });

      it('should call onPageChange when onChange of pagination is called', () => {
        wrapper.find('Pagination').simulate('change', 2, 24);
        expect(props.onPageChange).to.have.been.calledWith(2,24);
      });
    });

    context('when sorting is enabled', () => {
      beforeEach(() => {
        props.sortColumn = 'material_level_1_sort';
        props.sortOrder = 'Descending';
        props.onSortChange = sinon.spy();
        wrapper = shallow(<Table {...props} />);
      });
      it('should display short arrow on sortColumn', () => {
        expect(wrapper.find('th').first()).to.include.text('▾');
        expect(wrapper.find('th').at(1)).to.not.include.text('▾');
        expect(wrapper.find('th').at(2)).to.not.include.text('▾');
      });

      it('should display down short arrow on sortColumn when order is descending', () => {
        props.sortOrder = 'Ascending';
        wrapper = shallow(<Table {...props} />);
        expect(wrapper.find('th').first()).to.include.text('▴');
      });

      it('should display nothing if sortOrder is empty', () => {
        props.sortOrder = undefined;
        wrapper = shallow(<Table {...props} />);
        expect(wrapper.find('th').first()).to.not.include.text('▴');
        expect(wrapper.find('th').first()).to.not.include.text('▾');
      });

      it('should call onSortChange with the column and Ascending Order if it is not sorted on that column', () => {
        wrapper.find('th').at(1).simulate('click');
        expect(props.onSortChange).to.have.been.calledWith('rate_sort', 'Ascending');
      });
      it('should call onSortChange with the column and reverse sort Order if it is sorted on the same column', () => {
        wrapper.find('th').first().simulate('click');
        expect(props.onSortChange).to.have.been.calledWith('material_level_1_sort', 'Ascending');
      });
    });
  });
});
